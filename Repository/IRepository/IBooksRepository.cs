using RESTfulBookWebsite.Models;

namespace RESTfulBookWebsite.Repository.IRepository
{
    public interface IBooksRepository : IRepository<Book>
    {
        Task<Book> UpdateAsync(Book entity);

        Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorID);
        Task<IEnumerable<Book>> GetBooksByAuthorAsync(string authorName);
    }
}
