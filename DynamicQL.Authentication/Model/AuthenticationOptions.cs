using DynamicQL.Authentication.Enums;

namespace DynamicQL.Authentication.Model;

public abstract class AuthenticationOptions
{
    public virtual string AuthenticationQueryName { get; set; } = "authenticate";
    public virtual string RefreshTokenQueryName { get; set; } = "refreshToken";
    public virtual string InvalidateTokenQueryName { get; set; } = "invalidateToken";
    public virtual bool DoNotAddAuthenticationQuery { get; set; } = false;
    public virtual bool DoNotAddRefreshTokenQuery { get; set; } = false;
    public virtual bool DoNotAddInvalidateTokenQuery { get; set; } = false;
    public virtual List<PayloadKey> PayloadKeys { get; set; } = new ();
}