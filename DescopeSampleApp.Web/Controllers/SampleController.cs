using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DescopeSampleApp.Web.Controllers
{
    /// <summary>
    /// Sample API controller demonstrating JWT authentication
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly TokenValidator _tokenValidator;
        private readonly ILogger<SampleController> _logger;

        public SampleController(TokenValidator tokenValidator, ILogger<SampleController> logger)
        {
            _tokenValidator = tokenValidator;
            _logger = logger;
        }

        /// <summary>
        /// Sample GET endpoint that requires Bearer token authentication
        /// </summary>
        /// <returns>A sample message if authentication is successful</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Missing or invalid Authorization header");
                return Unauthorized(new { message = "Missing or invalid Authorization header" });
            }

            var sessionToken = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrEmpty(sessionToken))
            {
                _logger.LogWarning("Empty session token");
                return Unauthorized(new { message = "Empty session token" });
            }

            try
            {
                var claimsPrincipal = await _tokenValidator.ValidateSession(sessionToken);
                _logger.LogInformation("Successfully validated session token");

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
                return Unauthorized(new { message = "Token validation failed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token validation");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
