using DescopeSampleApp.Domain.Interfaces.Services;
using Jose;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace DescopeSampleApp.Infrastructure.Services;

/// <summary>
/// Service for validating Descope JWT tokens
/// </summary>
public class TokenValidationService : ITokenValidationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TokenValidationService> _logger;
    private readonly string _projectId;

    public TokenValidationService(
        IHttpClientFactory httpClientFactory,
        ILogger<TokenValidationService> logger,
        string projectId)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _projectId = Environment.GetEnvironmentVariable("DESCOPE_PROJECT_ID") ?? projectId;
    }

    public async Task<string> ValidateSessionAsync(string sessionToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var jwks = await GetPublicKeyAsync(_projectId, cancellationToken);

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
                    _logger.LogDebug(ex, "Failed to validate token with current JWK, trying next key");
                }
            }

            _logger.LogWarning("Failed to validate token with any JWK");
            throw new InvalidOperationException("Failed to validate token with any JWK.");
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
            var isValid = expirationTime > DateTimeOffset.UtcNow;

            _logger.LogInformation("Token expiration verification: {IsValid}", isValid);
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying token expiration");
            return false;
        }
    }

    private async Task<JwkSet> GetPublicKeyAsync(string projectId, CancellationToken cancellationToken)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://api.descope.com/{projectId}/.well-known/jwks.json";
            var keys = await client.GetStringAsync(url, cancellationToken);
            var jwks = JwkSet.FromJson(keys, JWT.DefaultSettings.JsonMapper);

            _logger.LogInformation("Successfully fetched public keys for project {ProjectId}", projectId);
            return jwks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching public key for project {ProjectId}", projectId);
            throw;
        }
    }
}
