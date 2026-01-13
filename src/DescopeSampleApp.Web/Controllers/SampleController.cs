using DescopeSampleApp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace DescopeSampleApp.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    private readonly ITokenValidatorService _tokenValidator;
    private readonly ILogger<SampleController> _logger;

    public SampleController(
        ITokenValidatorService tokenValidator,
        ILogger<SampleController> logger)
    {
        _tokenValidator = tokenValidator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        try
        {
            var authorizationHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Missing or invalid authorization header");
                return Unauthorized(new { message = "Missing or invalid authorization header" });
            }

            var sessionToken = authorizationHeader.Substring("Bearer ".Length).Trim();
            if (string.IsNullOrEmpty(sessionToken))
            {
                _logger.LogWarning("Empty session token");
                return Unauthorized(new { message = "Invalid session token" });
            }

            var claimsPrincipal = await _tokenValidator.ValidateSessionAsync(sessionToken, cancellationToken);
            _logger.LogInformation("Token validated successfully for user: {UserId}", claimsPrincipal.Identity?.Name);

            return Ok(new { message = "This is a sample API endpoint.", authenticated = true });
        }
        catch (SecurityException ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return Unauthorized(new { message = "Invalid or expired token" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in Sample API");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}
