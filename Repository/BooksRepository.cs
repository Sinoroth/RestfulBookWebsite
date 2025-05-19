using Microsoft.EntityFrameworkCore;
using RESTfulBookWebsite.Models;
using RESTfulBookWebsite.Repository.IRepository;
using UdemiCourse.Data;

namespace RESTfulBookWebsite.Repository
{
    public class BooksRepository: Repository<Book>, IBooksRepository
    {
        private readonly ApplicationDBContext _db;
        public BooksRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(string authorName)
        {
            var author = await _db.Authors
                          .Where(b => b.Name == authorName)
                          .FirstOrDefaultAsync();

            if (author == null)
            {
                return Enumerable.Empty<Book>(); 
            }

            return await GetBooksByAuthorIdAsync(author.Id);
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId)
        {
            return await _db.Books
                                 .Where(b => b.AuthorID == authorId)
                                 .ToListAsync();
        }

        public async Task<Book> UpdateAsync(Book entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _db.Books.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

    }
}
