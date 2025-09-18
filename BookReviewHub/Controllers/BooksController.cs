using Microsoft.AspNetCore.Mvc;

namespace BookReviewHub.Controllers
{
    public class BooksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
