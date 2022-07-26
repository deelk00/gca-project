using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Utility.Redis;

namespace Utility;

public static class Helper
{
    public static async Task<bool> IsTokenValid(string tokenString, RedisService redis, TokenValidationParameters param)
    {
        var token = ParseToken(tokenString);

        var jti = token?.Claims
            .FirstOrDefault(claim => claim.Type.ToLower() == "jti")
            ?.Value;

        if (jti == null) return false;

        var isValid = await redis.Get<bool?>(jti) 
                      ?? ValidateJwtTokenSignature(tokenString, param);

        return isValid;
    }

    private static bool ValidateJwtTokenSignature(string token, TokenValidationParameters param)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            tokenHandler.ValidateToken(token, param, out var validatedToken);

            return validatedToken.ValidTo < DateTime.Now
                   && validatedToken.ValidFrom > DateTime.Now;
        }
        catch(Exception ex)
        {
            return false;
        }
    }
    
    private static JwtSecurityToken? ParseToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(token);
        }
        catch(Exception ex)
        {
            return null;
        }
    }

    public static (string, string) CreateAuthTokenPair(
        byte[] privateKey, 
        string issuer,
        string audience,
        uint authExpiresIn, 
        uint refreshExpiresIn,
        IEnumerable<Claim> payload
    ) 
    {
        var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(privateKey, out _);
        var utcNow = DateTime.UtcNow;
        var authUtcEnd = utcNow.AddSeconds(authExpiresIn);
        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha512);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            payload,
            utcNow,
            authUtcEnd,
            signingCredentials
        );
        var handler = new JwtSecurityTokenHandler();

        var refreshUtcEnd = utcNow.AddSeconds(refreshExpiresIn);
        var refreshToken = new JwtSecurityToken(
            issuer,
            audience,
            new []{new Claim("jti", Guid.NewGuid().ToString())},
            utcNow,
            refreshUtcEnd,
            signingCredentials
            );
        return (handler.WriteToken(token), handler.WriteToken(refreshToken));
    }
}