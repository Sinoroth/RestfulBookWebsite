using Microsoft.AspNetCore.JsonPatch;
using RESTfulBookWebsite.Models.Dto;
using RESTfulBookWebsite.Response;

namespace RESTfulBookWebsite.Services.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUser(int id);
        Task<UserDTO> GetUser(string name);
        Task<bool> DoesUserExistForAuthor(int userID);
        Task<ServiceResponseObject<AuthorWithUserDTO>> GetAuthorWithUserAsync(int id);
        Task<ServiceResponseObject<UserDTO>> CreateUser(UserCreateDTO authorDTO);
        Task<ServiceResponseObject<UserDTO>> DeleteUser(int id);
        Task<ServiceResponseObject<UserDTO>> UpdateUserAsync(UserUpdateDTO authorDTO);
        Task<ServiceResponseObject<UserDTO>> UpdateUserPartialAsync(int id, JsonPatchDocument<UserDTO> patchDTO);
    }
}
