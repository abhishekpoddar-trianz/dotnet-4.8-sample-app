using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DescopeSampleApp.Web.Pages;

public class LoginModel : PageModel
{
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(ILogger<LoginModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation("Login page accessed");
    }
}
