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
    public class UserService : IUserService
    {
        private readonly IAuthorsRepository _authorsRepo;
        private readonly IBooksRepository _booksRepo;
        private readonly IUserRepository _usersRepo;
        private readonly IMapper _mapper;

        public UserService(IAuthorsRepository authorsRepo, IBooksRepository booksRepo, IUserRepository userRepo, IMapper mapper)
        {
            _authorsRepo = authorsRepo;
            _booksRepo = booksRepo;
            _usersRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<ServiceResponseObject<UserDTO>> CreateUser(UserCreateDTO userDTO)
        {
            var user = _mapper.Map<User>(userDTO);

            user.CreatedDate = DateTime.UtcNow;
            user.UpdatedDate = DateTime.UtcNow;

            try
            {
                await _usersRepo.CreateAsync(user);
            }
            catch (DbUpdateException ex)
            {
                return new ServiceResponseObject<UserDTO>
                {
                    Success = false,
                    ErrorMessage = $"Database update failed: {ex.Message}",
                    ErrorCode = 409
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponseObject<UserDTO>
                {
                    Success = false,
                    ErrorMessage = $"Unexpected error occurred: {ex.Message}",
                    ErrorCode = 500
                };
            }

            var savedUser = _mapper.Map<UserDTO>(user);

            return new ServiceResponseObject<UserDTO>
            {
                Success = true,
                Data = savedUser
            };
        }

        public async Task<ServiceResponseObject<UserDTO>> DeleteUser(int id)
        {
            var user = await _usersRepo.GetAsync(a => a.Id == id);

            if (user == null)
            {
                return new ServiceResponseObject<UserDTO>
                {
                    Success = false,
                    ErrorCode = 404,
                    ErrorMessage = $"User with ID {id} not found."
                };
            }

            try
            {
                await _usersRepo.RemoveAsync(user);
            }
            catch (Exception ex)
            {
                return new ServiceResponseObject<UserDTO>
                {
                    Success = false,
                    ErrorCode = 500,
                    ErrorMessage = $"Failed to delete user: {ex.Message}"
                };
            }

            return new ServiceResponseObject<UserDTO>
            {
                Success = true,
            };
        }

        public async Task<bool> DoesUserExistForAuthor(int userID)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _usersRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<UserDTO> GetUser(string name)
        {
            var user = await _usersRepo.GetAsync(u => u.Name.ToLower() == name.ToLower());

            if (user == null) { return null; }

            var userDTO = _mapper.Map<UserDTO>(user);

            return userDTO;
        }

        public Task<ServiceResponseObject<AuthorWithUserDTO>> GetAuthorWithUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDTO> GetUser(int id)
        {
            var user = await _usersRepo.GetAsync(u => u.Id == id);

            if (user == null) { return null; }

            var userDTO = _mapper.Map<UserDTO>(user);

            return userDTO;
        }

        public async Task<ServiceResponseObject<UserDTO>> UpdateUserAsync(UserUpdateDTO userDTO)
        {
            var existingUser = await _usersRepo.GetAsync(a => a.Id == userDTO.Id);

            if (existingUser == null)
            {
                return new ServiceResponseObject<UserDTO>
                {
                    Success = false,
                    ErrorCode = 404,
                    ErrorMessage = $"User with ID {userDTO.Id} not found."
                };
            }

            existingUser.Name = userDTO.Name;
            existingUser.UpdatedDate = DateTime.UtcNow;

            try
            {
                await _usersRepo.UpdateAsync(existingUser);
                var updatedDTO = _mapper.Map<UserDTO>(existingUser);

                return new ServiceResponseObject<UserDTO>
                {
                    Success = true,
                    Data = updatedDTO
                };
            }
            catch (DbUpdateException ex)
            {
                return new ServiceResponseObject<UserDTO>
                {
                    Success = false,
                    ErrorCode = 409,
                    ErrorMessage = $"Update failed: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponseObject<UserDTO>
                {
                    Success = false,
                    ErrorCode = 500,
                    ErrorMessage = $"An unexpected error occurred: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponseObject<UserDTO>> UpdateUserPartialAsync(int id, JsonPatchDocument<UserDTO> patchDTO)
        {
            var existingUser = await _usersRepo.GetAsync(a => a.Id == id);

            if (existingUser == null)
            {
                return new ServiceResponseObject<UserDTO>
                {
                    Success = false,
                    ErrorCode = 404,
                    ErrorMessage = $"Author with ID {id} not found."
                };
            }

            var userDTO = _mapper.Map<UserDTO>(existingUser);

            try
            {
                patchDTO.ApplyTo(userDTO);

                var updatedUser = _mapper.Map<User>(userDTO);

                updatedUser.UpdatedDate = DateTime.UtcNow;

                await _usersRepo.UpdateAsync(updatedUser);

                return new ServiceResponseObject<UserDTO>
                {
                    Success = true,
                    Data = _mapper.Map<UserDTO>(updatedUser)
                };
            }
            catch (JsonPatchException ex)
            {
                return new ServiceResponseObject<UserDTO>
                {
                    Success = false,
                    ErrorCode = 400,
                    ErrorMessage = $"Patch operation failed: {ex.Message}"
                };
            }
            catch (DbUpdateException ex)
            {
                return new ServiceResponseObject<UserDTO>
                {
                    Success = false,
                    ErrorCode = 409,
                    ErrorMessage = $"Update failed due to conflict: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponseObject<UserDTO>
                {
                    Success = false,
                    ErrorCode = 500,
                    ErrorMessage = $"An unexpected error occurred: {ex.Message}"
                };
            }
        }
    }
}
