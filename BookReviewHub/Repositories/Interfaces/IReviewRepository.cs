using System.Collections.Generic;
using System.Threading.Tasks;
using BookReviewHub.Models;

namespace BookReviewHub.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetAllAsync();
        Task<Review> GetByIdAsync(int id);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(int id);
        Task<IEnumerable<Review>> GetByBookIdAsync(int bookId);
        Task<bool> ExistsAsync(int id);
    }
}
