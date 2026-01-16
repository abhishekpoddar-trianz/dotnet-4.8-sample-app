namespace DescopeSampleApp.Domain.Interfaces.Services;

/// <summary>
/// Service interface for JWT token validation
/// </summary>
public interface ITokenValidationService
{
    /// <summary>
    /// Validates a session token and returns the payload
    /// </summary>
    Task<string> ValidateSessionAsync(string sessionToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies if a token has expired
    /// </summary>
    bool VerifyTokenExpiration(string sessionToken);
}
