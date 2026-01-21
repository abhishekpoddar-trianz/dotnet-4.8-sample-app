using DescopeSampleApp.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace DescopeSampleApp.Web.Controllers;

/// <summary>
/// Sample API controller demonstrating token validation
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    private readonly ITokenValidationService _tokenValidationService;
    private readonly ILogger<SampleController> _logger;

    public SampleController(
        ITokenValidationService tokenValidationService,
        ILogger<SampleController> logger)
    {
        _tokenValidationService = tokenValidationService;
        _logger = logger;
    }

    /// <summary>
    /// Sample GET endpoint that requires authentication
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        try
        {
            var authorizationHeader = Request.Headers.Authorization.ToString();

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(new { message = "Missing or invalid authorization header" });
            }

            var sessionToken = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrEmpty(sessionToken))
            {
                return Unauthorized(new { message = "Session token is required" });
            }

            var payload = await _tokenValidationService.ValidateSessionAsync(sessionToken, cancellationToken);

            _logger.LogInformation("Successfully validated token");

            return Ok(new {
                message = "This is a sample API endpoint.",
                authenticated = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return Unauthorized(new { message = "Invalid or expired token" });
        }
    }
}
