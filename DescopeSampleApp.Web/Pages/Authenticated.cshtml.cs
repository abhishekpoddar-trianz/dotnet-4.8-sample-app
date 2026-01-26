using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DescopeSampleApp.Web.Pages
{
    /// <summary>
    /// Authenticated page model - shows protected content
    /// </summary>
    public class AuthenticatedModel : PageModel
    {
        private readonly ILogger<AuthenticatedModel> _logger;

        public AuthenticatedModel(ILogger<AuthenticatedModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            _logger.LogInformation("Authenticated page accessed");
        }
    }
}
