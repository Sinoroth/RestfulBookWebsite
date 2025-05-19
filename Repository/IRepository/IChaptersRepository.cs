using RESTfulBookWebsite.Models;

namespace RESTfulBookWebsite.Repository.IRepository
{
    public interface IChaptersRepository : IRepository<Chapter>
    {
        Task<Chapter> UpdateAsync(Chapter entity);

    }
}
