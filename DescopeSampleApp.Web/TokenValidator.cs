using Jose;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

/// <summary>
/// Configuration class for Descope project settings
/// </summary>
public class Config
{
    public static string DescopeProjectId => Environment.GetEnvironmentVariable("DESCOPE_PROJECT_ID") ?? "P2dI0leWLEC45BDmfxeOCSSOWiCt";
}

/// <summary>
/// Service for validating JWT tokens issued by Descope
/// </summary>
public class TokenValidator
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TokenValidator> _logger;
    private readonly string _projectId;

    public TokenValidator(IHttpClientFactory httpClientFactory, ILogger<TokenValidator> logger, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _projectId = configuration["Descope:ProjectId"] ?? Config.DescopeProjectId;
    }

    /// <summary>
    /// Validates a session token using Descope's public keys
    /// </summary>
    /// <param name="sessionToken">The JWT token to validate</param>
    /// <returns>The decoded payload as a string</returns>
    /// <exception cref="Exception">Thrown when token validation fails</exception>
    public async Task<string> ValidateSession(string sessionToken)
    {
        try
        {
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
                    _logger.LogDebug(ex, "Failed to validate token with current JWK, trying next");
                }
            }

            _logger.LogWarning("Failed to validate token with any JWK");
            throw new Exception("Failed to validate token with any JWK.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing and verifying token");
            throw;
        }
    }

    /// <summary>
    /// Verifies if a token has expired
    /// </summary>
    /// <param name="sessionToken">The JWT token to check</param>
    /// <returns>True if the token is still valid, false otherwise</returns>
    public bool VerifyTokenExpiration(string sessionToken)
    {
        try
        {
            var payload = JWT.Payload<JObject>(sessionToken);
            var exp = payload["exp"];
            if (exp == null)
            {
                _logger.LogWarning("Token does not contain 'exp' claim");
                return false;
            }
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds((long)exp);
            var isValid = expirationTime > DateTimeOffset.UtcNow;

            if (!isValid)
            {
                _logger.LogWarning("Token has expired");
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying token expiration");
            return false;
        }
    }

    /// <summary>
    /// Retrieves the public key set from Descope's JWKS endpoint
    /// </summary>
    /// <param name="projectId">The Descope project ID</param>
    /// <returns>The JWK set containing public keys</returns>
    private async Task<JwkSet> GetPublicKeyAsync(string projectId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://api.descope.com/{projectId}/.well-known/jwks.json";

            _logger.LogDebug("Fetching public keys from {Url}", url);

            string keys = await client.GetStringAsync(url);
            JwkSet jwks = JwkSet.FromJson(keys, JWT.DefaultSettings.JsonMapper);

            _logger.LogDebug("Successfully retrieved {KeyCount} public keys", jwks.Keys.Count);

            return jwks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching public key from Descope");
            throw;
        }
    }
}
