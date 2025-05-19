using RESTfulBookWebsite.Models;
using RESTfulBookWebsite.Repository.IRepository;
using UdemiCourse.Data;

namespace RESTfulBookWebsite.Repository
{
    public class ChaptersRepository : Repository<Chapter>, IChaptersRepository
    {
        private readonly ApplicationDBContext _db;
        public ChaptersRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }
        public Task<Chapter> UpdateAsync(Chapter entity)
        {
            throw new NotImplementedException();
        }
    }
}
