namespace DynamicQL.Authentication.Model;

public class TokenResult
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public int ExpiresIn { get; set; }
}