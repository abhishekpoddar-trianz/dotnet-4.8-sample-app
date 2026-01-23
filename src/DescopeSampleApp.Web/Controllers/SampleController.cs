using DescopeSampleApp.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DescopeSampleApp.Web.Controllers;

/// <summary>
/// Sample API controller for demonstrating authentication
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
        _tokenValidationService = tokenValidationService ?? throw new ArgumentNullException(nameof(tokenValidationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Sample GET endpoint that requires authentication
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        try
        {
            var authorizationHeader = Request.Headers.Authorization.ToString();

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Missing or invalid authorization header");
                return Unauthorized(new { message = "Authorization header is missing or invalid" });
            }

            var sessionToken = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrEmpty(sessionToken))
            {
                _logger.LogWarning("Session token is empty");
                return Unauthorized(new { message = "Session token is empty" });
            }

            // Validate the session token
            try
            {
                var payload = await _tokenValidationService.ValidateSessionAsync(sessionToken, cancellationToken);
                _logger.LogInformation("Token validated successfully for user");

                return Ok(new
                {
                    message = "This is a sample API endpoint.",
                    authenticated = true,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (SecurityTokenValidationException ex)
            {
                _logger.LogWarning(ex, "Token validation failed");
                return Unauthorized(new { message = "Invalid or expired token" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token validation");
                return Unauthorized(new { message = "Token validation error" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in Sample API");
            return StatusCode(500, new { message = "An unexpected error occurred" });
        }
    }
}
