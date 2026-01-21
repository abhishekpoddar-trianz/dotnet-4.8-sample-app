using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DescopeSampleApp.Web.Services;

namespace DescopeSampleApp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    private readonly ITokenValidator _tokenValidator;
    private readonly ILogger<SampleController> _logger;

    public SampleController(ITokenValidator tokenValidator, ILogger<SampleController> logger)
    {
        _tokenValidator = tokenValidator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var authorizationHeader = Request.Headers["Authorization"].ToString();
        
        if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var sessionToken = authorizationHeader.Substring("Bearer ".Length).Trim();
            
            if (!string.IsNullOrEmpty(sessionToken))
            {
                try
                {
                    var validationResult = await _tokenValidator.ValidateSessionAsync(sessionToken);
                    _logger.LogInformation("Token validated successfully");
                    return Ok(new { message = "This is a sample API endpoint.", status = "authenticated" });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Token validation failed");
                    return Unauthorized(new { message = "Invalid token" });
                }
            }
        }

        _logger.LogWarning("No authorization header provided");
        return Unauthorized(new { message = "Authorization header required" });
    }
}
