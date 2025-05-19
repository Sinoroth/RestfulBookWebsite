using Microsoft.AspNetCore.JsonPatch;
using RESTfulBookWebsite.Models;
using RESTfulBookWebsite.Models.Dto;
using RESTfulBookWebsite.Response;

namespace RESTfulBookWebsite.Services.IServices
{
    public interface IBookService
    {
        Task<IEnumerable<BookDTO>> GetAllBooksAsync();
        Task<BookDTO> GetBook(int id);
        Task<BookDTO> GetBook(string name);
        Task<ServiceResponseObject<BookDTO>> CreateBook(BookCreateDTO authorDTO);
        Task<ServiceResponseObject<BookDTO>> DeleteBook(int id);
        Task<ServiceResponseObject<BookDTO>> UpdateBookAsync(BookDTO authorDTO);
        Task<ServiceResponseObject<BookDTO>> UpdateBookPartialAsync(int id, JsonPatchDocument<BookDTO> patchDTO);
        Task<ServiceResponseObject<IEnumerable<BookDTO>>> GetBooksByGenreAsync(Genre genre);
        Task<ServiceResponseObject<IEnumerable<BookDTO>>> GetBooksByAuthorAsync(string authorName);
        Task<bool> DoesBookHaveAuthor(int authorID);
    }
}
