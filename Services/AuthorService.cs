using AutoMapper;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using RESTfulBookWebsite.Models;
using RESTfulBookWebsite.Models.Dto;
using RESTfulBookWebsite.Repository;
using RESTfulBookWebsite.Repository.IRepository;
using RESTfulBookWebsite.Response;
using RESTfulBookWebsite.Services.IServices;

namespace RESTfulBookWebsite.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorsRepository _authorsRepo;
        private readonly IBooksRepository _booksRepo;
        private readonly IUserRepository _usersRepo;
        private readonly IMapper _mapper;

        public AuthorService(IAuthorsRepository authorsRepo, IBooksRepository booksRepo, IUserRepository userRepo, IMapper mapper)
        {
            _authorsRepo = authorsRepo;
            _booksRepo = booksRepo;
            _usersRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AuthorDTO>> GetAllAuthorsAsync()
        {
            var authors = await _authorsRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<AuthorDTO>>(authors);
        }

        public async Task<AuthorDTO> GetAuthor(int id)
        {
            var author = await _authorsRepo.GetAsync(u => u.Id == id);

            if (author == null) { return null; }

            var authorDTO = _mapper.Map<AuthorDTO>(author);

            return authorDTO;
        }

        public async Task<AuthorDTO> GetAuthor(string name)
        {
            var author = await _authorsRepo.GetAsync(u => u.Name.ToLower() == name.ToLower());

            if (author == null) { return null; }

            var authorDTO = _mapper.Map<AuthorDTO>(author);

            return authorDTO;
        }


        public async Task<AuthorWithBooksDTO?> GetAuthorWithBooksAsync(int authorId)
        {
            var author = await _authorsRepo.GetAsync(a => a.Id == authorId);

            if (author == null) return null;

            var books = await _booksRepo.GetBooksByAuthorIdAsync(authorId);

            var booksDTO = _mapper.Map<List<BookDTO>>(books);

            var authorDTO = _mapper.Map<AuthorWithBooksDTO>(author);

            authorDTO.Books = booksDTO;

            return authorDTO;
        }

        public async Task<bool> DoesUserExistForAuthor(int userID)
        {
            var user = await _usersRepo.GetAsync(u => u.Id == userID);

            if (user == null)
            {
                return false;
            }
            else return true;
        }

        public async Task<ServiceResponseObject<AuthorWithUserDTO>> GetAuthorWithUserAsync(int id)
        {
            var author = await _authorsRepo.GetAsync(u => u.Id == id, tracked: false);

            if (author == null)
            {
                return new ServiceResponseObject<AuthorWithUserDTO>
                {
                    Success = false,
                    ErrorCode = 404,
                    ErrorMessage = $"Author with ID {id} not found."
                };
            }

            var user = await _usersRepo.GetAsync(u => author.UserID == id, tracked: false);

            if (user == null)
            {
                return new ServiceResponseObject<AuthorWithUserDTO>
                {
                    Success = false,
                    ErrorCode = 404,
                    ErrorMessage = $"User with ID {author.UserID} (linked to Author {id}) not found."
                };
            }

            var userDTO = _mapper.Map<UserDTO>(user);

            var authorDTO = _mapper.Map<AuthorWithUserDTO>(author);
            
            authorDTO.User = userDTO;


            return new ServiceResponseObject<AuthorWithUserDTO>
            {
                Success = true,
                Data = authorDTO
            };
        }

        public async Task<ServiceResponseObject<AuthorDTO>> CreateAuthor(AuthorCreateDTO authorDTO)
        {
            var author = _mapper.Map<Author>(authorDTO);

            author.CreatedDate = DateTime.UtcNow;
            author.UpdatedDate = DateTime.UtcNow;

            try
            {
                await _authorsRepo.CreateAsync(author);
            }
            catch (DbUpdateException ex)
            {
                return new ServiceResponseObject<AuthorDTO>
                {
                    Success = false,
                    ErrorMessage = $"Database update failed: {ex.Message}",
                    ErrorCode = 409
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponseObject<AuthorDTO>
                {
                    Success = false,
                    ErrorMessage = $"Unexpected error occurred: {ex.Message}",
                    ErrorCode = 500
                };
            }

            var savedAuthor = _mapper.Map<AuthorDTO>(author);

            return new ServiceResponseObject<AuthorDTO>
            {
                Success = true,
                Data = savedAuthor
            };
        }

        public async Task<ServiceResponseObject<AuthorDTO>> DeleteAuthor(int id)
        {
            var author = await _authorsRepo.GetAsync(a => a.Id == id);

            if (author == null)
            {
                return new ServiceResponseObject<AuthorDTO>
                {
                    Success = false,
                    ErrorCode = 404,
                    ErrorMessage = $"Author with ID {id} not found."
                };
            }

            try
            {
                await _authorsRepo.RemoveAsync(author);
            }
            catch (Exception ex)
            {
                return new ServiceResponseObject<AuthorDTO>
                {
                    Success = false,
                    ErrorCode = 500,
                    ErrorMessage = $"Failed to delete author: {ex.Message}"
                };
            }

            return new ServiceResponseObject<AuthorDTO>
            {
                Success = true,
            };
        }

        public async Task<ServiceResponseObject<AuthorDTO>> UpdateAuthorAsync(AuthorUpdateDTO authorDTO)
        {
            var existingAuthor = await _authorsRepo.GetAsync(a => a.Id == authorDTO.Id);

            if (existingAuthor == null)
            {
                return new ServiceResponseObject<AuthorDTO>
                {
                    Success = false,
                    ErrorCode = 404,
                    ErrorMessage = $"Author with ID {authorDTO.Id} not found."
                };
            }

            existingAuthor.Name = authorDTO.Name;
            existingAuthor.UpdatedDate = DateTime.UtcNow;

            try
            {
                await _authorsRepo.UpdateAsync(existingAuthor);
                var updatedDTO = _mapper.Map<AuthorDTO>(existingAuthor);

                return new ServiceResponseObject<AuthorDTO>
                {
                    Success = true,
                    Data = updatedDTO
                };
            }
            catch (DbUpdateException ex)
            {
                return new ServiceResponseObject<AuthorDTO>
                {
                    Success = false,
                    ErrorCode = 409,
                    ErrorMessage = $"Update failed: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponseObject<AuthorDTO>
                {
                    Success = false,
                    ErrorCode = 500,
                    ErrorMessage = $"An unexpected error occurred: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponseObject<AuthorDTO>> UpdateAuthorPartialAsync(int id, JsonPatchDocument<AuthorDTO> patchDTO)
        {
            var existingAuthor = await _authorsRepo.GetAsync(a => a.Id == id);

            if (existingAuthor == null)
            {
                return new ServiceResponseObject<AuthorDTO>
                {
                    Success = false,
                    ErrorCode = 404,
                    ErrorMessage = $"Author with ID {id} not found."
                };
            }

            var authorDTO = _mapper.Map<AuthorDTO>(existingAuthor);

            try
            {
                patchDTO.ApplyTo(authorDTO);

                var updatedAuthor = _mapper.Map<Author>(authorDTO);

                updatedAuthor.UpdatedDate = DateTime.UtcNow;

                await _authorsRepo.UpdateAsync(updatedAuthor);

                return new ServiceResponseObject<AuthorDTO>
                {
                    Success = true,
                    Data = _mapper.Map<AuthorDTO>(updatedAuthor)
                };
            }
            catch (JsonPatchException ex)
            {
                return new ServiceResponseObject<AuthorDTO>
                {
                    Success = false,
                    ErrorCode = 400,
                    ErrorMessage = $"Patch operation failed: {ex.Message}"
                };
            }
            catch (DbUpdateException ex)
            {
                return new ServiceResponseObject<AuthorDTO>
                {
                    Success = false,
                    ErrorCode = 409,
                    ErrorMessage = $"Update failed due to conflict: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponseObject<AuthorDTO>
                {
                    Success = false,
                    ErrorCode = 500,
                    ErrorMessage = $"An unexpected error occurred: {ex.Message}"
                };
            }
        }
    }
}
