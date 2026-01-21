using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace DescopeSampleApp.Web.Services;

/// <summary>
/// Service for validating Descope JWT tokens
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
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
        _projectId = _configuration["Descope:ProjectId"]
            ?? Environment.GetEnvironmentVariable("DESCOPE_PROJECT_ID")
            ?? "P2dI0leWLEC45BDmfxeOCSSOWiCt";
    }

    public async Task<string> ValidateSessionAsync(string sessionToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var jwks = await GetPublicKeyAsync(_projectId, cancellationToken);

            if (jwks?.Keys == null || !jwks.Keys.Any())
            {
                throw new SecurityTokenValidationException("No public keys available");
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(sessionToken);

            foreach (var key in jwks.Keys)
            {
                try
                {
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = $"https://api.descope.com/{_projectId}",
                        ValidAudience = _projectId,
                        IssuerSigningKey = key
                    };

                    handler.ValidateToken(sessionToken, validationParameters, out var validatedToken);
                    return JsonSerializer.Serialize(jwtToken.Payload);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Failed to validate token with key {KeyId}", key.KeyId);
                }
            }

            throw new SecurityTokenValidationException("Failed to validate token with any available key");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            throw;
        }
    }

    public bool VerifyTokenExpiration(string sessionToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(sessionToken);
            return jwtToken.ValidTo > DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying token expiration");
            return false;
        }
    }

    private async Task<JsonWebKeySet?> GetPublicKeyAsync(string projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("DescopeClient");
            var url = $"{projectId}/.well-known/jwks.json";
            var response = await client.GetStringAsync(url, cancellationToken);
            return JsonWebKeySet.Create(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching public key");
            throw;
        }
    }
}
