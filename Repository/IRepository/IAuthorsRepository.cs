using RESTfulBookWebsite.Models;

namespace RESTfulBookWebsite.Repository.IRepository
{
    public interface IAuthorsRepository : IRepository<Author>
    {
        Task<Author> UpdateAsync(Author entity);

        
    }
}
