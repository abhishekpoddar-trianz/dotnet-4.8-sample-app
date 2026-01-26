using Microsoft.AspNetCore.Mvc;

namespace DescopeSampleApp.Web.Controllers
{
    /// <summary>
    /// Home controller for the application
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Displays the home page
        /// </summary>
        public IActionResult Index()
        {
            ViewData["Title"] = "Home Page";
            _logger.LogInformation("Home page accessed");
            return View();
        }

        /// <summary>
        /// Displays the error page
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
