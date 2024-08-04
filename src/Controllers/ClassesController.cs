using Microsoft.AspNetCore.Mvc;

namespace Miniblog.Core.Controllers
{
    public class ClassesController : Controller
    {
        [Route("/classes")]
        [OutputCache(Profile = "default")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
