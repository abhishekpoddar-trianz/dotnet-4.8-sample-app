using Microsoft.AspNetCore.Mvc;
using DescopeSampleApp.Web.Services;
using Microsoft.IdentityModel.Tokens;

namespace DescopeSampleApp.Web.Controllers;

/// <summary>
/// Sample API controller demonstrating token validation
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class SampleController : ControllerBase
{
    private readonly ITokenValidator _tokenValidator;
    private readonly ILogger<SampleController> _logger;

    public SampleController(ITokenValidator tokenValidator, ILogger<SampleController> logger)
    {
        _tokenValidator = tokenValidator;
        _logger = logger;
    }

    /// <summary>
    /// Sample endpoint that requires authentication
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var authorizationHeader = Request.Headers.Authorization.ToString();

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Missing or invalid authorization header");
                return Unauthorized(new { message = "Authorization header is required" });
            }

            var sessionToken = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrEmpty(sessionToken))
            {
                _logger.LogWarning("Empty session token");
                return Unauthorized(new { message = "Session token is required" });
            }

            // Validate the session token
            var payload = await _tokenValidator.ValidateSession(sessionToken);

            _logger.LogInformation("Successfully validated token for request");
            return Ok(new { message = "This is a sample API endpoint.", timestamp = DateTime.UtcNow });
        }
        catch (SecurityTokenValidationException ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return Unauthorized(new { message = "Invalid token" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt");
            return Unauthorized(new { message = "Unauthorized" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}
