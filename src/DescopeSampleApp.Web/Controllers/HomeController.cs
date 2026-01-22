using Microsoft.AspNetCore.Mvc;

namespace DescopeSampleApp.Web.Controllers;

/// <summary>
/// Home controller for MVC views
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Index page
    /// </summary>
    public IActionResult Index()
    {
        _logger.LogInformation("Home page accessed");
        ViewBag.Title = "Home Page";
        return View();
    }

    /// <summary>
    /// Error page
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
