namespace DescopeSampleApp.Web.Services;

/// <summary>
/// Interface for token validation service
/// </summary>
public interface ITokenValidator
{
    /// <summary>
    /// Validates a session token
    /// </summary>
    /// <param name="sessionToken">The JWT session token</param>
    /// <returns>The decoded payload as string</returns>
    Task<string> ValidateSession(string sessionToken);

    /// <summary>
    /// Verifies token expiration
    /// </summary>
    /// <param name="sessionToken">The JWT session token</param>
    /// <returns>True if token is valid and not expired</returns>
    bool VerifyTokenExpiration(string sessionToken);
}
