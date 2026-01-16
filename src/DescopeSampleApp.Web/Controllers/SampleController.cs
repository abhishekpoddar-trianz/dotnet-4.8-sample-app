using DescopeSampleApp.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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
        _tokenValidationService = tokenValidationService ?? throw new ArgumentNullException(nameof(tokenValidationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Sample GET endpoint that requires authentication
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var authorizationHeader = Request.Headers.Authorization.FirstOrDefault();

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(new { message = "Invalid authorization header format" });
            }

            var sessionToken = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrEmpty(sessionToken))
            {
                return Unauthorized(new { message = "Token is missing" });
            }

            // Validate the session token
            var claimsPrincipal = await _tokenValidationService.ValidateSessionAsync(sessionToken);

            return Ok(new { message = "This is a sample API endpoint." });
        }
        catch (SecurityTokenValidationException ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return Unauthorized(new { message = "Invalid token" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request");
            return StatusCode(500, new { message = "An error occurred processing the request" });
        }
    }
}
