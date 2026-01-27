using Jose;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace DescopeSampleApp.Infrastructure.Services;

/// <summary>
/// Token validation service for Descope authentication
/// </summary>
public class TokenValidator
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TokenValidator> _logger;
    private readonly string _projectId;

    public TokenValidator(IHttpClientFactory httpClientFactory, ILogger<TokenValidator> logger, string projectId)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _projectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
    }

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
                    return payload;
                }
                catch (Exception)
                {
                    // If decoding fails with this key, try the next one
                }
            }

            throw new Exception("Failed to validate token with any JWK.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing and verifying token");
            throw;
        }
    }

    public bool VerifyTokenExpiration(string sessionToken)
    {
        try
        {
            var payload = JWT.Payload<JObject>(sessionToken);
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds((long)payload["exp"]!);
            return expirationTime > DateTimeOffset.UtcNow;
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
            var client = _httpClientFactory.CreateClient();
            var url = $"https://api.descope.com/{projectId}/.well-known/jwks.json";
            string keys = await client.GetStringAsync(url);
            JwkSet jwks = JwkSet.FromJson(keys, JWT.DefaultSettings.JsonMapper);
            return jwks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching public key");
            throw;
        }
    }
}
