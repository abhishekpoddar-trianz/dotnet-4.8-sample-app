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

    public IActionResult Index()
    {
        _logger.LogInformation("Home page accessed");
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
