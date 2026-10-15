using Microsoft.AspNetCore.Mvc;

namespace Miniblog.Core.Controllers
{
    public class ContactController : Controller
    {
        [Route("/contact")]
        [OutputCache(Profile = "default")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
