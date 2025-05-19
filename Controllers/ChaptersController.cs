using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTfulBookWebsite.Controllers;
using RESTfulBookWebsite.Models.Dto;
using RESTfulBookWebsite.Repository.IRepository;
using RESTfulBookWebsite.Services.IServices;
using RESTfulBookWebsite.Services;
using UdemiCourse.Data;
using RESTfulBookWebsite.Response;

namespace RESTfulBookWebsite.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChaptersController : ControllerBase
    {
        private readonly ILogger<ChaptersController> logger;
        private readonly IChapterService _chapterService;

        public ChaptersController(IBooksRepository booksRepo, IChaptersRepository chaptersRepo, IReviewsRepository reviewsRepo, IUserRepository userRepo, ILogger<ChaptersController> _logger, IMapper mapper)
        {
            _chapterService = new ChapterService(booksRepo, chaptersRepo, reviewsRepo, mapper);
            logger = _logger;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ChapterDTO>>> GetChapters()
        {
            logger.LogInformation("Getting all Chapters");
            var chapters = await _chapterService.GetAllChaptersAsync();
            return Ok(chapters);
        }

        [HttpGet("{id:int}", Name = "GetChapter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ChapterDTO>> GetChapter(int id)
        {
            if (id == 0)
            {
                logger.LogError("Get Chapter error with ID of " + id);
                return BadRequest("Bad Id");
            }
            var chapter = await _chapterService.GetChapter(id);

            if (chapter == null)
            {
                return NotFound();
            }
            return Ok(chapter);
        }

        [HttpGet("{name}", Name = "GetChaptersByBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        
        public async Task<ActionResult<IEnumerable<ChapterDTO>>> GetChaptersByBook(string name)
        {
            if (name == "")
            {
                logger.LogError("Empty Book Name " + name);
                return BadRequest("Bad Id");
            }

            var chapters = await _chapterService.GetChaptersByBook(name);

            return Ok(chapters);
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ChapterDTO>> CreateChapter([FromBody] ChapterDTO chapterDTO)
        {
            if (chapterDTO == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _chapterService.GetChapter(chapterDTO.Title) != null)
            {
                return Conflict("Chapter already Exists!");
            }

            ServiceResponseObject<ChapterDTO> result = await _chapterService.CreateChapter(chapterDTO);

            if (!result.Success)
            {
                switch (result.ErrorCode)
                {
                    case 400: return BadRequest(result.ErrorMessage);
                    case 404: return NotFound(result.ErrorMessage);
                    case 409: return Conflict(result.ErrorMessage);
                    default: return StatusCode(500, result.ErrorMessage);
                }
            }

            var createdChapter = result.Data;

            return CreatedAtRoute("GetChapter", new { id = createdChapter.Id }, createdChapter);
        }

        [HttpDelete("{id:int}", Name = "DeleteChapter")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteChapter(int id)
        {
            if (id == 0) return BadRequest();

            var result = await _chapterService.DeleteChapter(id);

            if (!result.Success)
            {
                if (result.ErrorCode == 404)
                    return NotFound(result.ErrorMessage);

                return StatusCode(StatusCodes.Status500InternalServerError, result.ErrorMessage);
            }
            return NoContent();
        }
        [HttpPut("{id:int}", Name = "UpdateChapter")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateChapter(int id, [FromBody] ChapterDTO chapterDTO)
        {
            if (chapterDTO == null || id != chapterDTO.Id)
                return BadRequest("Author data is invalid or IDs do not match.");

            var result = await _chapterService.UpdateChapterAsync(chapterDTO);

            if (!result.Success)
            {
                switch (result.ErrorCode)
                {
                    case 404: return NotFound(result.ErrorMessage);
                    case 409: return Conflict(result.ErrorMessage);
                    default: return StatusCode(500, result.ErrorMessage);
                }
            }

            return NoContent();
        }
        [HttpPatch("{id:int}", Name = "UpdatePartialChapter")]
        public async Task<IActionResult> UpdatePartialBook(int id, JsonPatchDocument<ChapterDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
                return BadRequest("Invalid patch data or ID.");

            var result = await _chapterService.UpdateChapterPartialAsync(id, patchDTO);

            if (!result.Success)
            {
                switch (result.ErrorCode)
                {
                    case 400: return BadRequest(result.ErrorMessage);
                    case 404: return NotFound(result.ErrorMessage);
                    case 409: return Conflict(result.ErrorMessage);
                    default: return StatusCode(500, result.ErrorMessage);
                }
            }

            return NoContent();
        }
    }
}
