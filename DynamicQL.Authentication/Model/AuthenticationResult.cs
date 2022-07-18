using DynamicQL.Authentication.Enums;

namespace DynamicQL.Authentication.Model;

public class AuthenticationResult
{
    public bool IsAuthenticated { get; set; }
    public TokenResult? TokenResult { get; set; }
    public Dictionary<PayloadKey, object?> Payload { get; set; } = new ();
}