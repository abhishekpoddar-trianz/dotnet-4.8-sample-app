using System.Security.Claims;

namespace DescopeSampleApp.Web.Services;

/// <summary>
/// Interface for token validation service
/// </summary>
public interface ITokenValidationService
{
    Task<string> ValidateSessionAsync(string sessionToken, CancellationToken cancellationToken = default);
    bool VerifyTokenExpiration(string sessionToken);
}
