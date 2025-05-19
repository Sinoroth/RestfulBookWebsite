using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using RESTfulBookWebsite.Models.Dto;
using RESTfulBookWebsite.Models;
using UdemiCourse.Data;
using Microsoft.EntityFrameworkCore;
using RESTfulBookWebsite.Repository.IRepository;
using RESTfulBookWebsite.Repository;
using AutoMapper;
using Azure;
using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using RESTfulBookWebsite.Services.IServices;
using RESTfulBookWebsite.Services;
using static System.Reflection.Metadata.BlobBuilder;
using RESTfulBookWebsite.Response;

namespace RESTfulBookWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly ILogger<AuthorsController> logger;
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorsRepository authorsRepo, IBooksRepository booksRepo, IUserRepository userRepo, ILogger<AuthorsController> _logger, IMapper mapper)
        {
            _authorService = new AuthorService(authorsRepo, booksRepo, userRepo, mapper);
            logger = _logger;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AuthorDTO>>> GetAuthors()
        {
            logger.LogInformation("Getting all Authors");
            var authors = await _authorService.GetAllAuthorsAsync();
            return Ok(authors);
        }

        [HttpGet("{id:int}/books")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAuthorWithBooks(int id)
        {
            var authorWithBooksDTO = await _authorService.GetAuthorWithBooksAsync(id);

            if (authorWithBooksDTO == null)
                return NotFound($"Author with ID {id} not found.");

            if (authorWithBooksDTO.Books.Count() == 0)
            {
                logger.LogInformation($"Author with ID {id} has no books.");
            }

            return Ok(authorWithBooksDTO);
        }

        [HttpGet("{id:int}/user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAuthorAndUser(int id)
        {
            var result = await _authorService.GetAuthorWithUserAsync(id);

            //Author ID might not exist or Author might not be coupled to a User
            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return Ok(result.Data);
        }


        [HttpGet("{id:int}", Name = "GetAuthor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthorDTO>> GetAuthor(int id)
        {
            if (id == 0)
            {
                logger.LogError("Get Author error with ID of " + id);
                return BadRequest("Bad Id");
            }
            var author = await _authorService.GetAuthor(id);

            if (author == null)
            {
                return NotFound();
            }
            return Ok(author);
        }

        [HttpGet("{name}", Name = "GetAuthorByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<AuthorDTO>> GetAuthorByName(string name)
        {
            if (name == "")
            {
                logger.LogError("Empty Author Name " + name);
                return BadRequest("Bad Id");
            }
            var author = await _authorService.GetAuthor(name);

            if (author == null)
            {
                return NotFound();
            }
            return Ok(author);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthorDTO>> CreateAuthor([FromBody] AuthorCreateDTO AuthorCreateDTO)
        {
            if (AuthorCreateDTO == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _authorService.GetAuthor(AuthorCreateDTO.Name) != null)
            {
                return Conflict("Author already Exists!");
            }

            if(await _authorService.DoesUserExistForAuthor(AuthorCreateDTO.UserID) == false)
            {
                ModelState.AddModelError("ErrorMessages", "User doesn't exist!");
                return BadRequest(ModelState);
            }

            ServiceResponseObject<AuthorDTO> result = await _authorService.CreateAuthor(AuthorCreateDTO);

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

            var createdAuthor = result.Data;

            return CreatedAtRoute("GetAuthor", new { id = createdAuthor.Id }, createdAuthor);
        }

        [HttpDelete("{id:int}", Name = "DeleteAuthor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAuthorAsync(int id)
        {
            if (id == 0) return BadRequest();

            var result = await _authorService.DeleteAuthor(id);

            if (!result.Success)
            {
                if (result.ErrorCode == 404)
                    return NotFound(result.ErrorMessage);

                return StatusCode(StatusCodes.Status500InternalServerError, result.ErrorMessage);
            }

            return NoContent();
        }
        [HttpPut("{id:int}", Name = "UpdateAuthor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] AuthorUpdateDTO authorDTO)
        {
            if (authorDTO == null || id != authorDTO.Id)
                return BadRequest("Author data is invalid or IDs do not match.");

            var result = await _authorService.UpdateAuthorAsync(authorDTO);

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
        [HttpPatch("{id:int}", Name = "UpdatePartialAuthor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePartialAuthor(int id, JsonPatchDocument<AuthorDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
                return BadRequest("Invalid patch data or ID.");

            var result = await _authorService.UpdateAuthorPartialAsync(id, patchDTO);

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
