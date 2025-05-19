using RESTfulBookWebsite.Models;
using RESTfulBookWebsite.Models.Dto;
using RESTfulBookWebsite.Repository.IRepository;
using UdemiCourse.Data;

namespace RESTfulBookWebsite.Repository
{
    public class AuthorsRepository : Repository<Author>, IAuthorsRepository
    {
        private readonly ApplicationDBContext _db;
        public AuthorsRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }


        public async Task<Author> UpdateAsync(Author entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _db.Authors.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}
