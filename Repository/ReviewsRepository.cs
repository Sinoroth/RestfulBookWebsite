using RESTfulBookWebsite.Models;
using RESTfulBookWebsite.Repository.IRepository;
using UdemiCourse.Data;

namespace RESTfulBookWebsite.Repository
{
    public class ReviewsRepository : Repository<Review>, IReviewsRepository
    {
        private readonly ApplicationDBContext _db;
        public ReviewsRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public Task<Review> UpdateAsync(Review entity)
        {
            throw new NotImplementedException();
        }
    }
}
