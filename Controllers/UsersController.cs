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
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> logger;
        private readonly IUserService _userService;

        public UsersController(IAuthorsRepository authorsRepo, IBooksRepository booksRepo, IUserRepository userRepo, ILogger<UsersController> _logger, IMapper mapper)
        {
            _userService = new UserService(authorsRepo, booksRepo, userRepo, mapper);
            logger = _logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            logger.LogInformation("Getting all Users");
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id:int}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            if (id == 0)
            {
                logger.LogError("Get Author error with ID of " + id);
                return BadRequest("Bad Id");
            }
            var user = await _userService.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("{name}", Name = "GetUserByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        
        public async Task<ActionResult<UserDTO>> GetUserByName(string name)
        {
            if (name == "")
            {
                logger.LogError("Empty User Name " + name);
                return BadRequest("Bad Id");
            }
            var user = await _userService.GetUser(name);

            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> CreateUser([FromBody] UserCreateDTO userCreateDTO)
        {
            if (userCreateDTO == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _userService.GetUser(userCreateDTO.Name) != null)
            {
                return Conflict("User already Exists!");
            }

            ServiceResponseObject<UserDTO> result = await _userService.CreateUser(userCreateDTO);

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

            return CreatedAtRoute("GetUser", new { id = createdAuthor.Id }, createdAuthor);
        }

        [HttpDelete("{id:int}", Name = "DeleteUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id == 0) return BadRequest();

            var result = await _userService.DeleteUser(id);

            if (!result.Success)
            {
                if (result.ErrorCode == 404)
                    return NotFound(result.ErrorMessage);

                return StatusCode(StatusCodes.Status500InternalServerError, result.ErrorMessage);
            }

            return NoContent();
        }
        [HttpPut("{id:int}", Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDTO userDTO)
        {
            if (userDTO == null || id != userDTO.Id)
                return BadRequest("User data is invalid or IDs do not match.");

            var result = await _userService.UpdateUserAsync(userDTO);

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
        [HttpPatch("{id:int}", Name = "UpdatePartialUser")]
        public async Task<IActionResult> UpdatePartialUser(int id, JsonPatchDocument<UserDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
                return BadRequest("Invalid patch data or ID.");

            var result = await _userService.UpdateUserPartialAsync(id, patchDTO);

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
