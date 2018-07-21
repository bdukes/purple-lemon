using Microsoft.AspNetCore.Mvc;

namespace Miniblog.Core.Controllers
{
    public class GalleryController : Controller
    {
        [Route("/galleries")]
        [Route("/gallery")]
        public IActionResult Index()
        {
            return RedirectToActionPermanent(nameof(Wedding));
        }

        [Route("/galleries/wedding")]
        [OutputCache(Profile = "default")]
        public IActionResult Wedding()
        {
            return View();
        }

        [Route("/galleries/weddings")]
        [Route("/weddings")]
        [Route("/wedding")]
        public IActionResult WeddingRedirects()
        {
            return RedirectToActionPermanent(nameof(Wedding));
        }

        [Route("/galleries/engagement-couples")]
        [OutputCache(Profile = "default")]
        public IActionResult EngagementCouples()
        {
            return View();
        }

        [Route("/galleries/engagement")]
        [Route("/galleries/engagements")]
        [Route("/galleries/couple")]
        [Route("/galleries/couples")]
        [Route("/engagement")]
        [Route("/engagements")]
        [Route("/couple")]
        [Route("/couples")]
        [Route("/engagement-couples")]
        public IActionResult EngagementRedirects()
        {
            return RedirectToActionPermanent(nameof(EngagementCouples));
        }

        [Route("/galleries/family")]
        [OutputCache(Profile = "default")]
        public IActionResult Family()
        {
            return View();
        }

        [Route("/galleries/families")]
        [Route("/families")]
        [Route("/family")]
        public IActionResult FamilyRedirects()
        {
            return RedirectToActionPermanent(nameof(Family));
        }

        [Route("/galleries/portraits")]
        [OutputCache(Profile = "default")]
        public IActionResult Portraits()
        {
            return View();
        }

        [Route("/galleries/portrait")]
        [Route("/galleries/senior")]
        [Route("/galleries/seniors")]
        [Route("/portrait")]
        [Route("/senior")]
        [Route("/seniors")]
        [Route("/portraits")]
        public IActionResult PortraitsRedirects()
        {
            return RedirectToActionPermanent(nameof(Portraits));
        }
    }
}
