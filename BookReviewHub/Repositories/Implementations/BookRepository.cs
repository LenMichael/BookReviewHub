using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookReviewHub.Models;
using BookReviewHub.Data;
using BookReviewHub.Repositories.Interfaces;

namespace BookReviewHub.Repositories.Implementations
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;

        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.Include(b => b.Reviews).ToListAsync();
        }

        public async Task<Book> GetByIdAsync(int id)
        {
            return await _context.Books.Include(b => b.Reviews).ThenInclude(r => r.ReviewVotes)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Book>> GetFilteredAsync(string genre, int? year, int? rating)
        {
            var query = _context.Books.Include(b => b.Reviews).AsQueryable();

            if (!string.IsNullOrEmpty(genre))
                query = query.Where(b => b.Genre == genre);

            if (year.HasValue)
                query = query.Where(b => b.PublishedYear == year.Value);

            if (rating.HasValue)
                query = query.Where(b => b.Reviews.Any(r => r.Rating == rating.Value));

            return await query.ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Books.AnyAsync(b => b.Id == id);
        }
    }
}
