using System.Security.Claims;
using Jose;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace DescopeSampleApp.Infrastructure.Services;

public interface ITokenValidatorService
{
    Task<ClaimsPrincipal> ValidateSessionAsync(string sessionToken, CancellationToken cancellationToken = default);
    bool VerifyTokenExpiration(string sessionToken);
}

public class TokenValidatorService : ITokenValidatorService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TokenValidatorService> _logger;
    private readonly string _projectId;

    public TokenValidatorService(
        IHttpClientFactory httpClientFactory,
        ILogger<TokenValidatorService> logger,
        string projectId)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _projectId = projectId;
    }

    public async Task<ClaimsPrincipal> ValidateSessionAsync(string sessionToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var jwks = await GetPublicKeyAsync(_projectId, cancellationToken);

            foreach (var jwk in jwks.Keys)
            {
                try
                {
                    var payload = JWT.Decode(sessionToken, jwk);
                    var claims = ParseClaims(payload);
                    var identity = new ClaimsIdentity(claims, "Descope");
                    return new ClaimsPrincipal(identity);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Failed to validate token with JWK, trying next key");
                }
            }

            throw new SecurityException("Failed to validate token with any JWK");
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
            var payload = JWT.Payload<JObject>(sessionToken);
            if (payload["exp"] != null)
            {
                var expValue = payload["exp"];
                if (expValue != null)
                {
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds((long)expValue);
                    return expirationTime > DateTimeOffset.UtcNow;
                }
            }
            return false;
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
            return jwks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching public key from Descope");
            throw;
        }
    }

    private List<Claim> ParseClaims(string payload)
    {
        var claims = new List<Claim>();
        var json = JObject.Parse(payload);

        foreach (var property in json.Properties())
        {
            claims.Add(new Claim(property.Name, property.Value.ToString()));
        }

        return claims;
    }
}

public class SecurityException : Exception
{
    public SecurityException(string message) : base(message) { }
}
