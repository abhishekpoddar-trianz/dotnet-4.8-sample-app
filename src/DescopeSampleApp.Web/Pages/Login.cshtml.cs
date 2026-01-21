using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DescopeSampleApp.Web.Pages;

public class LoginModel : PageModel
{
    private readonly IConfiguration _configuration;

    public string DescopeProjectId { get; set; } = string.Empty;

    public LoginModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void OnGet()
    {
        DescopeProjectId = _configuration["Descope:ProjectId"] ?? "P2dI0leWLEC45BDmfxeOCSSOWiCt";
    }
}
