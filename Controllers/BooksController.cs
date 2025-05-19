using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTfulBookWebsite.Controllers;
using RESTfulBookWebsite.Models.Dto;
using RESTfulBookWebsite.Repository.IRepository;
using RESTfulBookWebsite.Response;
using RESTfulBookWebsite.Services;
using RESTfulBookWebsite.Services.IServices;
using UdemiCourse.Data;

namespace RESTfulBookWebsite.Models
{
 
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ILogger<BooksController> logger;
        private readonly IBookService _bookService;

        public BooksController(IAuthorsRepository authorsRepo, IBooksRepository booksRepo, IChaptersRepository chapterRepo, IReviewsRepository reviewsRepository, ILogger<BooksController> _logger, IMapper mapper)
        {
            _bookService = new BookService(authorsRepo, booksRepo, chapterRepo, reviewsRepository, mapper);
            logger = _logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks()
        {
            logger.LogInformation("Getting all Books");
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet]
        [Route("GetBooksByGenre")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByGenre(string genre)
        {
            Genre genreProvided;
            try
            {
                Enum.TryParse(genre, out genreProvided);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }

            var result = await _bookService.GetBooksByGenreAsync(genreProvided);

            if (!result.Success)
                return NotFound(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet]
        [Route("GetByAuthorName")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByAuthor(string authorName)
        {
            var result = await _bookService.GetBooksByAuthorAsync(authorName);

            if (result == null)
            {
                return NotFound($"No books found for author '{authorName}'.");
            }

            if(!result.Data.Any())
            {
                return NotFound($"No books found for author '{authorName}'.");
            }

            return Ok(result);
        }

        [HttpGet("{id}", Name = "GetBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDTO>> GetBook(int id)
        {
            var book = await _bookService.GetBook(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        [HttpGet("ByName/{name}", Name = "GetBookByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        
        public async Task<ActionResult<BookDTO>> GetBookByName(string name)
        {
            if (name == "")
            {
                logger.LogError("Empty Book Name " + name);
                return BadRequest("Bad Id");
            }
            var book = await _bookService.GetBook(name);

            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookDTO>> CreateBook([FromBody] BookCreateDTO bookCreateDTO)
        {
            if (bookCreateDTO == null)
            {
                return BadRequest(bookCreateDTO);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _bookService.GetBook(bookCreateDTO.Name) != null)
            {
                return Conflict("Book already Exists!");
            }

            if (await _bookService.DoesBookHaveAuthor(bookCreateDTO.AuthorID) == false)
            {
                ModelState.AddModelError("ErrorMessages", "Author doesn't exist!");
                return BadRequest(ModelState);
            }

            ServiceResponseObject<BookDTO> result = await _bookService.CreateBook(bookCreateDTO);

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

            var createdBook = result.Data;

            return CreatedAtRoute("GetBook", new { id = createdBook.Id }, createdBook);
        }

        [HttpDelete("{id:int}", Name = "DeleteBook")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (id == 0) return BadRequest();

            var result = await _bookService.DeleteBook(id);

            if (!result.Success)
            {
                if (result.ErrorCode == 404)
                    return NotFound(result.ErrorMessage);

                return StatusCode(StatusCodes.Status500InternalServerError, result.ErrorMessage);
            }
            return NoContent();
        }
        [HttpPut("{id:int}", Name = "UpdateBook")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookDTO bookDTO)
        {
            if (bookDTO == null || bookDTO.Id != id) { return BadRequest(); }

            if (!Enum.TryParse(bookDTO.Genre, out Genre genre))
            {
                return BadRequest($"Invalid genre: {bookDTO.Genre}. Please provide a valid genre.");
            }
            var result = await _bookService.UpdateBookAsync(bookDTO);

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
        [HttpPatch("{id:int}", Name = "UpdatePartialBook")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePartialBook(int id, JsonPatchDocument<BookDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
                return BadRequest("Invalid patch data or ID.");

            var result = await _bookService.UpdateBookPartialAsync(id, patchDTO);

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
