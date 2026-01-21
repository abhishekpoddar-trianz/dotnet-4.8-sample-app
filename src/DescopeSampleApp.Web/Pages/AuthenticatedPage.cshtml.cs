using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DescopeSampleApp.Web.Pages;

/// <summary>
/// Page model for Authenticated Page
/// </summary>
public class AuthenticatedPageModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticatedPageModel> _logger;

    public AuthenticatedPageModel(IConfiguration configuration, ILogger<AuthenticatedPageModel> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string ProjectId { get; set; } = string.Empty;

    public void OnGet()
    {
        ProjectId = _configuration["Descope:ProjectId"] ?? "P2dI0leWLEC45BDmfxeOCSSOWiCt";
        _logger.LogInformation("Authenticated page accessed");
    }
}
