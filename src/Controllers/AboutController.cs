using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Miniblog.Core.Services;

namespace Miniblog.Core.Controllers
{
    public class AboutController : Controller
    {
        private readonly IBlogService _blog;

        public AboutController(IBlogService blog)
        {
            _blog = blog;
        }

        [Route("/about")]
        [OutputCache(Profile = "default")]
        public async Task<IActionResult> Index()
        {
            var posts = await _blog.GetPostsByCategory("purple lemon");
            ViewData["RelatedPosts"] = posts.ToArray();
            return View();
        }
    }
}
