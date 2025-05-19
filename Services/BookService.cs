using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTfulBookWebsite.Models;
using RESTfulBookWebsite.Models.Dto;
using RESTfulBookWebsite.Repository.IRepository;
using RESTfulBookWebsite.Response;
using RESTfulBookWebsite.Services.IServices;

namespace RESTfulBookWebsite.Services
{
    public class BookService : IBookService
    {
        private readonly IAuthorsRepository _authorsRepo;
        private readonly IBooksRepository _booksRepo;
        private readonly IChaptersRepository _chaptersRepo;
        private readonly IReviewsRepository _reviewsRepo;
        private readonly IMapper _mapper;

        public BookService(IAuthorsRepository authorsRepo, IBooksRepository booksRepo, IChaptersRepository chaptersRepo, IReviewsRepository reviewsRepo, IMapper mapper)
        {
            _authorsRepo = authorsRepo;
            _booksRepo = booksRepo;
            _chaptersRepo = chaptersRepo;
            _reviewsRepo = reviewsRepo;
            _mapper = mapper;
        }

        public async Task<ServiceResponseObject<BookDTO>> CreateBook(BookCreateDTO bookCreateDTO)
        {
            Book book = _mapper.Map<Book>(bookCreateDTO);

            book.CreatedDate = DateTime.UtcNow;
            book.UpdatedDate = DateTime.UtcNow;

            try
            {
                await _booksRepo.CreateAsync(book);
            }
            catch (DbUpdateException ex)
            {
                return new ServiceResponseObject<BookDTO>
                {
                    Success = false,
                    ErrorMessage = $"Database update failed: {ex.Message}",
                    ErrorCode = 409
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponseObject<BookDTO>
                {
                    Success = false,
                    ErrorMessage = $"Unexpected error occurred: {ex.Message}",
                    ErrorCode = 500
                };
            }

            var savedBook = _mapper.Map<BookDTO>(book);

            return new ServiceResponseObject<BookDTO>
            {
                Success = true,
                Data = savedBook
            };
        }

        public async Task<ServiceResponseObject<BookDTO>> DeleteBook(int id)
        {
            var book = await _booksRepo.GetAsync(a => a.Id == id);

            if (book == null)
            {
                return new ServiceResponseObject<BookDTO>
                {
                    Success = false,
                    ErrorCode = 404,
                    ErrorMessage = $"Author with ID {id} not found."
                };
            }

            try
            {
                await _booksRepo.RemoveAsync(book);
            }
            catch (Exception ex)
            {
                return new ServiceResponseObject<BookDTO>
                {
                    Success = false,
                    ErrorCode = 500,
                    ErrorMessage = $"Failed to delete author: {ex.Message}"
                };
            }

            return new ServiceResponseObject<BookDTO>
            {
                Success = true,
            };
        }

        public async Task<IEnumerable<BookDTO>> GetAllBooksAsync()
        {
            var books = await _booksRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<BookDTO>>(books);
        }

        public async Task<ServiceResponseObject<IEnumerable<BookDTO>>> GetBooksByGenreAsync(Genre genre)
        {
            var books = await _booksRepo.GetAllAsync(b => b.Genre == genre);

            if (books == null || !books.Any())
            {
                return new ServiceResponseObject<IEnumerable<BookDTO>>
                {
                    Success = false,
                    ErrorCode = 404,
                    ErrorMessage = $"No books found in genre '{genre}'."
                };
            }

            var bookDTOs = _mapper.Map<IEnumerable<BookDTO>>(books);

            return new ServiceResponseObject<IEnumerable<BookDTO>>
            {
                Success = true,
                Data = bookDTOs
            };
        }

        public async Task<ServiceResponseObject<IEnumerable<BookDTO>>> GetBooksByAuthorAsync(string authorName)
        {
            var books = await _booksRepo.GetBooksByAuthorAsync(authorName);

            if (books == null || !books.Any())
            {
                return null;  
            }
            var bookDTOs = _mapper.Map<IEnumerable<BookDTO>>(books);

            return new ServiceResponseObject<IEnumerable<BookDTO>>
            {
                Success = true,
                Data = bookDTOs
            };
        }

        public async Task<BookDTO> GetBook(int id)
        {
            var book = await _booksRepo.GetAsync(u => u.Id == id);

            if (book == null) { return null; }

            var bookDTO = _mapper.Map<BookDTO>(book);

            return bookDTO;
        }

        public async Task<BookDTO> GetBook(string name)
        {
            var book = await _booksRepo.GetAsync(u => u.Name.ToLower() == name.ToLower());

            if (book == null) { return null; }

            var bookDTO = _mapper.Map<BookDTO>(book);

            return bookDTO;
        }

        public async Task<ServiceResponseObject<BookDTO>> UpdateBookAsync(BookDTO bookDTO)
        {

            var existingBook = await _booksRepo.GetAsync(a => a.Id == bookDTO.Id);

            if (existingBook == null)
            {
                return new ServiceResponseObject<BookDTO>
                {
                    Success = false,
                    ErrorCode = 404,
                    ErrorMessage = $"Book with ID {bookDTO.Id} not found."
                };
            }

            existingBook.Name = bookDTO.Name;
            existingBook.UpdatedDate = DateTime.UtcNow;

            try
            {
                await _booksRepo.UpdateAsync(existingBook);
                var updatedDTO = _mapper.Map<BookDTO>(existingBook);

                return new ServiceResponseObject<BookDTO>
                {
                    Success = true,
                    Data = updatedDTO
                };
            }
            catch (DbUpdateException ex)
            {
                return new ServiceResponseObject<BookDTO>
                {
                    Success = false,
                    ErrorCode = 409,
                    ErrorMessage = $"Update failed: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponseObject<BookDTO>
                {
                    Success = false,
                    ErrorCode = 500,
                    ErrorMessage = $"An unexpected error occurred: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponseObject<BookDTO>> UpdateBookPartialAsync(int id, JsonPatchDocument<BookDTO> patchDTO)
        {
            var existingBook = await _booksRepo.GetAsync(a => a.Id == id);

            if (existingBook == null)
            {
                return new ServiceResponseObject<BookDTO>
                {
                    Success = false,
                    ErrorCode = 404,
                    ErrorMessage = $"Book with ID {id} not found."
                };
            }

            var bookDTO = _mapper.Map<BookDTO>(existingBook);

            try
            {
                patchDTO.ApplyTo(bookDTO);

                var updatedBook = _mapper.Map<Book>(bookDTO);
                updatedBook.UpdatedDate = DateTime.UtcNow;

                await _booksRepo.UpdateAsync(updatedBook);

                return new ServiceResponseObject<BookDTO>
                {
                    Success = true,
                    Data = _mapper.Map<BookDTO>(updatedBook)
                };
            }
            catch (JsonPatchException ex)
            {
                return new ServiceResponseObject<BookDTO>
                {
                    Success = false,
                    ErrorCode = 400,
                    ErrorMessage = $"Patch operation failed: {ex.Message}"
                };
            }
            catch (DbUpdateException ex)
            {
                return new ServiceResponseObject<BookDTO>
                {
                    Success = false,
                    ErrorCode = 409,
                    ErrorMessage = $"Update failed due to conflict: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponseObject<BookDTO>
                {
                    Success = false,
                    ErrorCode = 500,
                    ErrorMessage = $"An unexpected error occurred: {ex.Message}"
                };
            }
        }

        public async Task<bool> DoesBookHaveAuthor(int authorID)
        {
            var user = await _authorsRepo.GetAsync(u => u.Id == authorID);

            if (user == null)
            {
                return false;
            }
            else return true;
        }
    }
}
