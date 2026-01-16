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
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IActionResult Index()
    {
        _logger.LogInformation("Home page accessed");
        ViewData["Title"] = "Home Page";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
