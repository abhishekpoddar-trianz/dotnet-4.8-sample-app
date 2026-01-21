namespace DescopeSampleApp.Web.Services;

public interface ITokenValidator
{
    Task<string> ValidateSessionAsync(string sessionToken);
    bool VerifyTokenExpiration(string sessionToken);
}
