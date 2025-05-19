using RESTfulBookWebsite.Models;
using RESTfulBookWebsite.Repository.IRepository;
using UdemiCourse.Data;

namespace RESTfulBookWebsite.Repository
{
    public class UsersRepository : Repository<User>, IUserRepository
    {
        private readonly ApplicationDBContext _db;
        public UsersRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public bool IsUniqueUser(string username)
        {
            var user = _db.Users.FirstOrDefault(x => x.Username == username);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<User> UpdateAsync(User entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _db.Users.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

    }
}
