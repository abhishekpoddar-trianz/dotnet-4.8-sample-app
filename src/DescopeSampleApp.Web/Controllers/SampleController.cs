using DescopeSampleApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
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
        _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        try
        {
            var authorizationHeader = Request.Headers.Authorization.FirstOrDefault();
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                _logger.LogWarning("Missing authorization header");
                return Unauthorized(new { message = "Authorization header is missing" });
            }

            var token = authorizationHeader.Replace("Bearer ", string.Empty);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Invalid authorization header format");
                return Unauthorized(new { message = "Invalid authorization header format" });
            }

            var principal = await _tokenValidator.ValidateSessionAsync(token, cancellationToken);
            if (principal == null)
            {
                _logger.LogWarning("Token validation failed");
                return Unauthorized(new { message = "Invalid or expired token" });
            }

            _logger.LogInformation("API request successful");
            return Ok(new { message = "This is a sample API endpoint." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing API request");
            return StatusCode(500, new { message = "An error occurred processing your request" });
        }
    }
}
