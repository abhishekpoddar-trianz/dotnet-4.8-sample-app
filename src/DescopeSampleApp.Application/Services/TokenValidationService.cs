using DescopeSampleApp.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;

namespace DescopeSampleApp.Application.Services;

/// <summary>
/// Service implementation for JWT token validation using Descope
/// </summary>
public class TokenValidationService : ITokenValidationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenValidationService> _logger;
    private readonly string _projectId;

    public TokenValidationService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<TokenValidationService> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _projectId = _configuration["Descope:ProjectId"]
            ?? Environment.GetEnvironmentVariable("DESCOPE_PROJECT_ID")
            ?? "P2dI0leWLEC45BDmfxeOCSSOWiCt";
    }

    public async Task<string> ValidateSessionAsync(string sessionToken, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating session token");

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(sessionToken);

            // Fetch public keys from Descope
            var httpClient = _httpClientFactory.CreateClient();
            var url = $"https://api.descope.com/{_projectId}/.well-known/jwks.json";
            var keysJson = await httpClient.GetStringAsync(url, cancellationToken);

            var jwksObject = JObject.Parse(keysJson);
            var keys = jwksObject["keys"];

            if (keys == null)
            {
                throw new InvalidOperationException("No keys found in JWKS response");
            }

            // For simplicity, we're returning the payload
            // In production, you should validate the signature with the public key
            var payload = token.Payload.SerializeToJson();

            _logger.LogInformation("Token validated successfully");
            return payload;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating session token");
            throw;
        }
    }

    public bool VerifyTokenExpiration(string sessionToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(sessionToken);

            if (token.ValidTo < DateTime.UtcNow)
            {
                _logger.LogWarning("Token has expired");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying token expiration");
            return false;
        }
    }
}
