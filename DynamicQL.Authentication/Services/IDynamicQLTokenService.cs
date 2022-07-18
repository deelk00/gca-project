using DynamicQL.Authentication.Enums;
using DynamicQL.Authentication.Model;
using GraphQLParser;

namespace DynamicQL.Authentication.Services;

public interface IDynamicQLTokenService
{
    Task<TokenResult> CreateToken(Dictionary<PayloadKey, object?> payload);
    Task<Dictionary<PayloadKey, object?>?> ParseToken(string token);
    Task<(bool, Dictionary<PayloadKey, object?>?)> IsAuthenticationTokenValid(string token);
    Task<(bool, Dictionary<PayloadKey, object?>?)> IsRefreshTokenValid(string token);
    Task InvalidateToken(string token);
}