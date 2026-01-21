using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Http;

namespace DescopeSampleApp.Infrastructure.Services;

/// <summary>
/// Service for validating Descope JWT tokens
/// </summary>
public class TokenValidationService
{
    private readonly HttpClient _httpClient;
    private readonly string _projectId;
    private readonly ILogger<TokenValidationService> _logger;

    public TokenValidationService(
        IHttpClientFactory httpClientFactory,
        ILogger<TokenValidationService> logger,
        string projectId)
    {
        _httpClient = httpClientFactory.CreateClient();
        _projectId = projectId;
        _logger = logger;
    }

    /// <summary>
    /// Validates a Descope session token
    /// </summary>
    public async Task<string> ValidateSessionAsync(string sessionToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var jwks = await GetPublicKeysAsync(_projectId, cancellationToken);

            foreach (var key in jwks.Keys)
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = true,
                        ValidIssuer = $"https://api.descope.com/{_projectId}",
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };

                    var principal = handler.ValidateToken(sessionToken, validationParameters, out var validatedToken);
                    var jwtToken = validatedToken as JwtSecurityToken;

                    if (jwtToken != null)
                    {
                        return JsonSerializer.Serialize(jwtToken.Payload);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug("Failed to validate token with key: {Message}", ex.Message);
                }
            }

            throw new SecurityTokenValidationException("Failed to validate token with any JWK.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            throw;
        }
    }

    /// <summary>
    /// Verifies if a token is expired
    /// </summary>
    public bool VerifyTokenExpiration(string sessionToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(sessionToken);
            return token.ValidTo > DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying token expiration");
            return false;
        }
    }

    private async Task<JsonWebKeySet> GetPublicKeysAsync(string projectId, CancellationToken cancellationToken)
    {
        try
        {
            var url = $"https://api.descope.com/{projectId}/.well-known/jwks.json";
            var response = await _httpClient.GetStringAsync(url, cancellationToken);
            return new JsonWebKeySet(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching public keys");
            throw;
        }
    }
}
