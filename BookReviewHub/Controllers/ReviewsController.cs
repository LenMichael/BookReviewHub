using Microsoft.AspNetCore.Mvc;

namespace BookReviewHub.Controllers
{
    public class ReviewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
