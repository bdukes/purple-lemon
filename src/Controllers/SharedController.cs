namespace Miniblog.Core.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class SharedController : Controller
    {
        [Route("{*url}", Order = int.MaxValue)]
        public IActionResult RouteNotFound() => View("Error", 404);

        public IActionResult Error() => this.View(this.Response.StatusCode);

        /// <summary>
        /// This is for use in wwwroot/serviceworker.js to support offline scenarios
        /// </summary>
        public IActionResult Offline() => this.View();
    }
}
