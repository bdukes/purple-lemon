using Microsoft.AspNetCore.Mvc;

namespace Miniblog.Core.Controllers
{
    public class HomeController : Controller
    {
        [Route("/")]
        [OutputCache(Profile = "default")]
        public IActionResult Index()
        {
            ViewData["PageImageUrl"] = "https://res.cloudinary.com/purple-lemon-photography/image/upload/c_crop,f_auto,fl_lossy,g_south_east,h_553,q_auto,w_829/v1544557509/home-preview";
            ViewData["PageImageWidth"] = "829";
            ViewData["PageImageHeight"] = "553";
            return View();
        }
    }
}
