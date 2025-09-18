using BookReviewHub.Data;
using BookReviewHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookReviewHub.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/books?genre=...&year=...&rating=...
        [HttpGet]
        public async Task<IActionResult> GetBooks([FromQuery] string? genre, [FromQuery] int? year, [FromQuery] int? rating)
        {
            var books = _context.Books.Include(b => b.Reviews).AsQueryable();

            if (!string.IsNullOrEmpty(genre))
                books = books.Where(b => b.Genre == genre);

            if (year.HasValue)
                books = books.Where(b => b.PublishedYear == year.Value);

            if (rating.HasValue)
                books = books.Where(b => b.Reviews.Any(r => r.Rating == rating.Value));

            var result = await books
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Author,
                    b.PublishedYear,
                    b.Genre,
                    AverageRating = b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0
                })
                .ToListAsync();

            return Ok(result);
        }

        // GET: api/books/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _context.Books
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            return Ok(new
            {
                book.Id,
                book.Title,
                book.Author,
                book.PublishedYear,
                book.Genre,
                Reviews = book.Reviews.Select(r => new
                {
                    r.Id,
                    r.Content,
                    r.Rating,
                    r.DateCreated,
                    r.UserId
                })
            });
        }

        // POST: api/books
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateBook([FromBody] BookCreateDto bookDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool exists = await _context.Books.AnyAsync(b => b.Title == bookDto.Title && b.Author == bookDto.Author);

            if (exists)
                return BadRequest("A book with the same title and author already exists.");

            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                PublishedYear = bookDto.PublishedYear,
                Genre = bookDto.Genre
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // GET: api/books/5/reviews
        [HttpGet("{id}/reviews")]
        public async Task<IActionResult> GetBookReviews(int id)
        {
            var book = await _context.Books
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            var reviews = book.Reviews.Select(r => new
            {
                r.Id,
                r.Content,
                r.Rating,
                r.DateCreated,
                r.UserId
            });

            return Ok(reviews);
        }
    }
}
