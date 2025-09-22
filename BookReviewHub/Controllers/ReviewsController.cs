using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookReviewHub.Models;
using BookReviewHub.Repositories.Interfaces;
using BookReviewHub.Dtos;

namespace BookReviewHub.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookRepository _bookRepository;

        public ReviewsController(IReviewRepository reviewRepository, IBookRepository bookRepository)
        {
            _reviewRepository = reviewRepository;
            _bookRepository = bookRepository;
        }

        // GET: Reviews/Index
        public async Task<IActionResult> Index()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            return View(reviews);
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var review = await _reviewRepository.GetByIdAsync(id.Value);
            if (review == null)
                return NotFound();

            return View(review);
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                var book = await _bookRepository.GetByIdAsync(dto.BookId);
                ViewBag.ValidationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View("~/Views/Books/Reviews.cshtml", book);
            }

            var review = new Review
            {
                Content = dto.Content,
                Rating = dto.Rating,
                BookId = dto.BookId,
                UserId = userId,
                DateCreated = DateTime.Now
            };

            await _reviewRepository.AddAsync(review);
            return RedirectToAction("Reviews", "Books", new { id = dto.BookId });
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var review = await _reviewRepository.GetByIdAsync(id.Value);
            if (review == null)
                return NotFound();

            return View(review);
        }

        // POST: Reviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,Rating,DateCreated,BookId,UserId")] Review review)
        {
            if (id != review.Id)
                return NotFound();

            var existingReview = await _reviewRepository.GetByIdAsync(id);
            if (existingReview == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(review);

            review.UserId = existingReview.UserId;
            await _reviewRepository.UpdateAsync(review);
            return RedirectToAction(nameof(Index));
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var review = await _reviewRepository.GetByIdAsync(id.Value);
            if (review == null)
                return NotFound();

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _reviewRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
