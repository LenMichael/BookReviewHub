using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookReviewHub.Data;
using BookReviewHub.Models;

namespace BookReviewHub
{
    public class ReviewsController : Controller
    {
        private readonly AppDbContext _context;

        public ReviewsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Reviews.Include(r => r.Book);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create (disabled)
        [HttpGet]
        public IActionResult Create()
        {
            return NotFound();
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Content,Rating,BookId")] Review review)
        {
            if (!ModelState.IsValid)
                return View(review);
            review.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            review.DateCreated = DateTime.Now;
            _context.Add(review);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Books", new { id = review.BookId });
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Author", review.BookId);
            return View(review);
        }

        // POST: Reviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,Rating,DateCreated,BookId,UserId")] Review review)
        {
            if (id != review.Id)
                return NotFound();

            var existingReview = await _context.Reviews.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            if (existingReview == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewData["BookId"] = new SelectList(_context.Books, "Id", "Author", review.BookId);
                return View(review);
            }

            review.UserId = existingReview.UserId;

            try
            {
                _context.Update(review);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(review.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(int id, bool isUpvote)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return NotFound();

            var existingVote = await _context.ReviewVotes
                .FirstOrDefaultAsync(v => v.ReviewId == id && v.UserId == userId);

            if (existingVote != null)
            {
                existingVote.IsUpvote = isUpvote;
            }
            else
            {
                var vote = new ReviewVote
                {
                    ReviewId = id,
                    UserId = userId,
                    IsUpvote = isUpvote
                };
                _context.ReviewVotes.Add(vote);
            }

            await _context.SaveChangesAsync();

            var reviewEntity = await _context.Reviews.FindAsync(id);
            return RedirectToAction("Reviews", "Books", new { id = reviewEntity.BookId });
        }

    }
}
