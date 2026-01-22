using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DescopeSampleApp.Web.Pages;

/// <summary>
/// Authenticated page model
/// </summary>
public class AuthenticatedPageModel : PageModel
{
    private readonly ILogger<AuthenticatedPageModel> _logger;

    public AuthenticatedPageModel(ILogger<AuthenticatedPageModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation("Authenticated page accessed");
    }
}
