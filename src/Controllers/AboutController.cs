using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Miniblog.Core.Services;

namespace Miniblog.Core.Controllers
{
    public class AboutController : Controller
    {
        private readonly IBlogService blog;

        public AboutController(IBlogService blog)
        {
            this.blog = blog;
        }

        [Route("/about")]
        [OutputCache(Profile = "default")]
        public IActionResult Index()
        {
            var posts = this.blog.GetPostsByCategory("purple lemon");
            ViewData["RelatedPosts"] = posts.TakeLast(3);
            return View();
        }
    }
}
