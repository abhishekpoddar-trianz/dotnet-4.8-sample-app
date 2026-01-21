using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace DescopeSampleApp.Web.Services;

public class TokenValidator : ITokenValidator
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _projectId;
    private readonly ILogger<TokenValidator> _logger;

    public TokenValidator(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<TokenValidator> logger)
    {
        _httpClientFactory = httpClientFactory;
        _projectId = configuration["Descope:ProjectId"] ?? "P2dI0leWLEC45BDmfxeOCSSOWiCt";
        _logger = logger;
    }

    public async Task<string> ValidateSessionAsync(string sessionToken)
    {
        try
        {
            var jwks = await GetPublicKeysAsync(_projectId);
            
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
                        IssuerSigningKey = key,
                        ClockSkew = TimeSpan.Zero
                    };

                    var handler = new JwtSecurityTokenHandler();
                    var principal = handler.ValidateToken(sessionToken, validationParameters, out var validatedToken);
                    
                    return JsonSerializer.Serialize(new { success = true, user = principal.Identity?.Name });
                }
                catch (SecurityTokenException)
                {
                    continue;
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

    private async Task<JsonWebKeySet> GetPublicKeysAsync(string projectId)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://api.descope.com/{projectId}/.well-known/jwks.json";
            var keysJson = await client.GetStringAsync(url);
            return new JsonWebKeySet(keysJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching public keys");
            throw;
        }
    }
}
