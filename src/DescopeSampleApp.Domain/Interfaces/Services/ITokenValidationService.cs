namespace DescopeSampleApp.Domain.Interfaces.Services;

/// <summary>
/// Interface for token validation service
/// </summary>
public interface ITokenValidationService
{
    /// <summary>
    /// Validates a session token and returns the payload
    /// </summary>
    /// <param name="sessionToken">The JWT session token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The decoded token payload</returns>
    Task<string> ValidateSessionAsync(string sessionToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies if a token has expired
    /// </summary>
    /// <param name="sessionToken">The JWT session token</param>
    /// <returns>True if token is still valid, false if expired</returns>
    bool VerifyTokenExpiration(string sessionToken);
}
