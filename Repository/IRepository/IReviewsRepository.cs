using RESTfulBookWebsite.Models;

namespace RESTfulBookWebsite.Repository.IRepository
{
    public interface IReviewsRepository : IRepository<Review>
    {
        Task<Review> UpdateAsync(Review entity);

    }
}
