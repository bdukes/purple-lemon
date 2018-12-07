using Microsoft.AspNetCore.Mvc;

namespace Miniblog.Core.Controllers
{
    public class HomeController : Controller
    {
        [Route("/")]
        [OutputCache(Profile = "default")]
        public IActionResult Index()
        {
            ViewData["PageImageUrl"] = "https://res.cloudinary.com/purple-lemon-photography/image/upload/f_auto,fl_keep_attribution.lossy,q_auto/v1540088577/Couples/De_Angelo_Beth-035";
            return View();
        }
    }
}
