using System.Collections.Generic;
using System.Threading.Tasks;
using BookReviewHub.Models;

namespace BookReviewHub.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book> GetByIdAsync(int id);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(int id);
        Task<IEnumerable<Book>> GetFilteredAsync(string genre, int? year, int? rating);
        Task<bool> ExistsAsync(int id);
    }
}
