using Microsoft.AspNetCore.Mvc;

namespace BookReviewHub.Controllers.Api
{
    public class BooksApiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
