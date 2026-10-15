using Microsoft.AspNetCore.Mvc;

namespace Miniblog.Core.Controllers
{
    public class InvestmentController : Controller
    {
        [Route("/investment")]
        [OutputCache(Profile = "default")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
