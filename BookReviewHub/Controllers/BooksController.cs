using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookReviewHub.Data;
using BookReviewHub.Models;
using BookReviewHub.Repositories.Interfaces;

namespace BookReviewHub.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IActionResult> Index(string genre, int? year, int? rating)
        {
            var books = await _bookRepository.GetFilteredAsync(genre, year, rating);
            return View(books);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var book = await _bookRepository.GetByIdAsync(id.Value);
            if (book == null)
                return NotFound();

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,PublishedYear,Genre")] BookCreateDto bookDto)
        {
            if (!ModelState.IsValid)
                return View(bookDto);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                PublishedYear = bookDto.PublishedYear,
                Genre = bookDto.Genre,
                UserId = userId
            };

            await _bookRepository.AddAsync(book);
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var book = await _bookRepository.GetByIdAsync(id.Value);
            if (book == null)
                return NotFound();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (book.UserId != userId)
                return Unauthorized();

            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,PublishedYear,Genre")] Book book)
        {
            if (id != book.Id)
                return NotFound();

            var dbBook = await _bookRepository.GetByIdAsync(id);
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (dbBook == null || dbBook.UserId != userId)
                return Unauthorized();

            if (!ModelState.IsValid)
                return View(book);

            book.UserId = userId;
            await _bookRepository.UpdateAsync(book);
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var book = await _bookRepository.GetByIdAsync(id.Value);
            if (book == null)
                return NotFound();

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (book.UserId != userId)
                return Unauthorized();

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (book != null && book.UserId == userId)
                _bookRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> BookExists(int id)
        {
            return await _bookRepository.ExistsAsync(id);
        }

        public async Task<IActionResult> Reviews(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
                return NotFound();

            return View(book);
        }

    }
}
