using RESTfulBookWebsite.Models;
using RESTfulBookWebsite.Models.Dto;

namespace RESTfulBookWebsite.Repository.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        bool IsUniqueUser(string username);
        Task<User> UpdateAsync(User entity);

        //TO DO: Login and Register
    }
}
