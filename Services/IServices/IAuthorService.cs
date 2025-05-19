using Microsoft.AspNetCore.JsonPatch;
using RESTfulBookWebsite.Models.Dto;
using RESTfulBookWebsite.Response;

namespace RESTfulBookWebsite.Services.IServices
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDTO>> GetAllAuthorsAsync();
        Task<AuthorDTO> GetAuthor(int id);
        Task<AuthorDTO> GetAuthor(string name);
        Task<bool> DoesUserExistForAuthor(int userID);
        Task<AuthorWithBooksDTO?> GetAuthorWithBooksAsync(int authorId);
        Task<ServiceResponseObject<AuthorWithUserDTO>> GetAuthorWithUserAsync(int id);
        Task<ServiceResponseObject<AuthorDTO>> CreateAuthor(AuthorCreateDTO authorDTO);
        Task<ServiceResponseObject<AuthorDTO>> DeleteAuthor(int id);
        Task<ServiceResponseObject<AuthorDTO>> UpdateAuthorAsync(AuthorUpdateDTO authorDTO);
        Task<ServiceResponseObject<AuthorDTO>> UpdateAuthorPartialAsync(int id, JsonPatchDocument<AuthorDTO> patchDTO);

    }
}
