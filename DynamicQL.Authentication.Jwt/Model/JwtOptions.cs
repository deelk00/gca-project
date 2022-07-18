using Microsoft.IdentityModel.Tokens;

namespace DynamicQL.Authentication.Jwt.Model;

public class JwtOptions
{
    public int KeySize { get; set; } = 2048;
    public byte[]? PrivateKey { get; set; }
    public byte[]? PublicKey { get; set; }

    public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha512Signature;
    
    public int TokenExpirationTime { get; set; } = 900;
    public int RefreshTokenExpirationTime { get; set; } = 60*60*24*7;
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public int ClockSkewInSeconds { get; set; } = 0;
}