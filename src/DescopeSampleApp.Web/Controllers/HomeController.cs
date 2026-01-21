using Microsoft.AspNetCore.Mvc;

namespace DescopeSampleApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Home Page";
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
