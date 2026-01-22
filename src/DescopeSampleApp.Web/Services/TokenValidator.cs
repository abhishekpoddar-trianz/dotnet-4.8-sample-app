using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Jose;
using Newtonsoft.Json.Linq;

namespace DescopeSampleApp.Web.Services;

/// <summary>
/// Service for validating JWT tokens from Descope
/// </summary>
public class TokenValidator : ITokenValidator
{
    private readonly HttpClient _httpClient;
    private readonly string _projectId;
    private readonly ILogger<TokenValidator> _logger;

    public TokenValidator(string projectId, ILogger<TokenValidator>? logger = null)
    {
        _httpClient = new HttpClient();
        _projectId = projectId;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<TokenValidator>.Instance;
    }

    /// <summary>
    /// Validates a session token against Descope JWKS
    /// </summary>
    public async Task<string> ValidateSession(string sessionToken)
    {
        try
        {
            _logger.LogInformation("Validating session token");
            var jwks = await GetPublicKeyAsync(_projectId);

            foreach (var jwk in jwks.Keys)
            {
                try
                {
                    var payload = JWT.Decode(sessionToken, jwk);
                    _logger.LogInformation("Token validated successfully");
                    return payload;
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Failed to validate token with current key, trying next");
                }
            }

            _logger.LogWarning("Failed to validate token with any JWK");
            throw new UnauthorizedAccessException("Failed to validate token with any JWK.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing and verifying token");
            throw;
        }
    }

    /// <summary>
    /// Verifies if token is not expired
    /// </summary>
    public bool VerifyTokenExpiration(string sessionToken)
    {
        try
        {
            var payload = JWT.Payload<JObject>(sessionToken);
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds((long)payload["exp"]!);
            var isValid = expirationTime > DateTimeOffset.UtcNow;

            _logger.LogInformation("Token expiration check: {IsValid}", isValid);
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying token expiration");
            return false;
        }
    }

    private async Task<JwkSet> GetPublicKeyAsync(string projectId)
    {
        try
        {
            var url = $"https://api.descope.com/{projectId}/.well-known/jwks.json";
            _logger.LogDebug("Fetching JWKS from {Url}", url);

            string keys = await _httpClient.GetStringAsync(url);
            JwkSet jwks = JwkSet.FromJson(keys, JWT.DefaultSettings.JsonMapper);

            _logger.LogInformation("Successfully fetched JWKS");
            return jwks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching public key");
            throw;
        }
    }
}
