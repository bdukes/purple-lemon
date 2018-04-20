using Microsoft.AspNetCore.Mvc;

namespace Miniblog.Core.Controllers
{
    public class GalleryController : Controller
    {
        [Route("/galleries")]
        [OutputCache(Profile = "default")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
