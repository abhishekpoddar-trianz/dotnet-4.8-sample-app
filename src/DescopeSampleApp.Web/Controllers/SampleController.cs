using DescopeSampleApp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DescopeSampleApp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    private readonly TokenValidationService _tokenValidationService;
    private readonly ILogger<SampleController> _logger;

    public SampleController(
        TokenValidationService tokenValidationService,
        ILogger<SampleController> logger)
    {
        _tokenValidationService = tokenValidationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var authorizationHeader = Request.Headers.Authorization.FirstOrDefault();

        if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var sessionToken = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (!string.IsNullOrEmpty(sessionToken))
            {
                try
                {
                    var payload = await _tokenValidationService.ValidateSessionAsync(sessionToken, cancellationToken);
                    _logger.LogInformation("Token validated successfully");
                    return Ok(new { message = "This is a sample API endpoint.", authenticated = true });
                }
                catch (SecurityTokenValidationException ex)
                {
                    _logger.LogWarning(ex, "Token validation failed");
                    return Unauthorized(new { message = "Invalid token" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error validating token");
                    return StatusCode(500, new { message = "Internal server error" });
                }
            }
        }

        _logger.LogWarning("No authorization header provided");
        return Unauthorized(new { message = "Authorization header missing" });
    }
}
