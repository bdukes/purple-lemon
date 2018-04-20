using Microsoft.AspNetCore.Mvc;

namespace Miniblog.Core.Controllers
{
    public class AboutController : Controller
    {
        [Route("/about")]
        [OutputCache(Profile = "default")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
