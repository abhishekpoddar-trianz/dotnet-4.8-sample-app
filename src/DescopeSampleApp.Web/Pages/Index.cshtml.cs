using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DescopeSampleApp.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IndexModel> _logger;

    public string ProjectId { get; set; } = string.Empty;

    public IndexModel(IConfiguration configuration, ILogger<IndexModel> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public void OnGet()
    {
        ProjectId = _configuration["Descope:ProjectId"] ?? "P2dI0leWLEC45BDmfxeOCSSOWiCt";
        _logger.LogInformation("Index page accessed");
    }
}
