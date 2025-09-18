using BookReviewHub.Data;
using BookReviewHub.Dtos;
using BookReviewHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookReviewHub.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewsApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/reviews/book/{bookId}
        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetReviewsForBook(int bookId)
        {
            var book = await _context.Books
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
                return NotFound("Book not found.");

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


        // POST: api/reviews
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] ReviewCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var book = await _context.Books.FindAsync(dto.BookId);
            if (book == null)
                return NotFound("Book not found.");

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var review = new Review
            {
                Content = dto.Content,
                Rating = dto.Rating,
                BookId = dto.BookId,
                UserId = userId,
                DateCreated = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                review.Id,
                review.Content,
                review.Rating,
                review.DateCreated,
                review.BookId,
                review.UserId
            });
        }

        // POST: api/reviews/{id}/vote
        [HttpPost("{id}/vote")]
        [Authorize]
        public async Task<IActionResult> VoteReview(int id, [FromBody] ReviewVoteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return NotFound("Review not found.");

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var existingVote = await _context.ReviewVotes
                .FirstOrDefaultAsync(v => v.ReviewId == id && v.UserId == userId);

            if (existingVote != null)
            {
                existingVote.IsUpvote = dto.IsUpvote;
            }
            else
            {
                var vote = new ReviewVote
                {
                    ReviewId = id,
                    UserId = userId,
                    IsUpvote = dto.IsUpvote
                };
                _context.ReviewVotes.Add(vote);
            }

            await _context.SaveChangesAsync();
            return Ok(new { Success = true });
        }
    }
}
