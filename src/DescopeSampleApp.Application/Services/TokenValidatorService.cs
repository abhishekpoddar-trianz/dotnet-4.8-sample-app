using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace DescopeSampleApp.Application.Services;

public interface ITokenValidatorService
{
    Task<ClaimsPrincipal?> ValidateSessionAsync(string sessionToken, CancellationToken cancellationToken = default);
    bool IsTokenExpired(string sessionToken);
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
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _projectId = projectId ?? throw new ArgumentNullException(nameof(projectId));
    }

    public async Task<ClaimsPrincipal?> ValidateSessionAsync(string sessionToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(sessionToken);

            var httpClient = _httpClientFactory.CreateClient();
            var url = $"https://api.descope.com/{_projectId}/.well-known/jwks.json";
            var response = await httpClient.GetStringAsync(url, cancellationToken);
            var jwks = new JsonWebKeySet(response);

            foreach (var key in jwks.Keys)
            {
                try
                {
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = $"https://api.descope.com/{_projectId}",
                        ValidateAudience = true,
                        ValidAudience = _projectId,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };

                    var principal = handler.ValidateToken(sessionToken, validationParameters, out var validatedToken);
                    _logger.LogInformation("Token validated successfully");
                    return principal;
                }
                catch (Exception)
                {
                    continue;
                }
            }

            _logger.LogWarning("Failed to validate token with any JWK");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            throw;
        }
    }

    public bool IsTokenExpired(string sessionToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(sessionToken);
            return jwtToken.ValidTo < DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking token expiration");
            return true;
        }
    }
}
