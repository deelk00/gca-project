using DynamicQL.Authentication.Enums;
using DynamicQL.Authentication.Model;
using DynamicQL.Services;

namespace DynamicQL.Authentication.Services;

public interface IDynamicQLAuthenticationService
{
    Task<AuthenticationResult> Authenticate(string username, string password, List<PayloadKey> payloadKeys);
    Task<AuthenticationResult> RefreshToken(string token);
    Task Invalidate(string token);
}