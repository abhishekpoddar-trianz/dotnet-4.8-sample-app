using DescopeSampleApp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DescopeSampleApp.Web.Controllers;

/// <summary>
/// Sample API controller with token authentication
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    private readonly ILogger<SampleController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public SampleController(
        ILogger<SampleController> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var authorizationHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Missing or invalid authorization header");
                return Unauthorized();
            }

            var sessionToken = authorizationHeader.Substring("Bearer ".Length).Trim();
            if (string.IsNullOrEmpty(sessionToken))
            {
                _logger.LogWarning("Empty session token");
                return Unauthorized();
            }

            // Validate the session token
            var projectId = _configuration["Descope:ProjectId"] ?? "P2dI0leWLEC45BDmfxeOCSSOWiCt";
            var loggerFactory = HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
            var tokenValidatorLogger = loggerFactory.CreateLogger<TokenValidator>();
            var tokenValidator = new TokenValidator(_httpClientFactory, tokenValidatorLogger, projectId);

            try
            {
                var claimsPrincipal = await tokenValidator.ValidateSession(sessionToken);
                _logger.LogInformation("Token validated successfully");
                return Ok(new { message = "This is a sample API endpoint.", data = "Authenticated successfully" });
            }
            catch (SecurityTokenValidationException ex)
            {
                _logger.LogWarning(ex, "Token validation failed");
                return Unauthorized();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}
