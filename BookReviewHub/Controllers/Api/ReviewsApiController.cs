using Microsoft.AspNetCore.Mvc;

namespace BookReviewHub.Controllers.Api
{
    public class ReviewsApiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
