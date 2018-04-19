using Microsoft.AspNetCore.Mvc;

namespace Miniblog.Core.Controllers
{
    public class HomeController : Controller
    {
        [Route("/")]
        [OutputCache(Profile = "default")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
