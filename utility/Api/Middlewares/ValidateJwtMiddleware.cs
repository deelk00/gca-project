using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Utility.Redis;

namespace Utility.Api.Middlewares;

public class ValidateJwtMiddleware : IMiddleware
{
    private readonly SymmetricSecurityKey _publicKey;
    private readonly string _audience;
    private readonly string _issuer;
    
    private readonly RedisService _redis;
    
    public ValidateJwtMiddleware(RedisService redis, IConfiguration config)
    {
        _redis = redis;

        _audience = config.GetValue<string>("Jwt:Audience");
        _issuer = config.GetValue<string>("Jwt:Issuer");
        var pk = config.GetValue<string>("Jwt:PublicKey");
        _publicKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(pk));
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var auth = context.Request.Headers.Authorization.ToString().ToLower();

        if (auth.StartsWith("bearer "))
        {
            auth = auth[7..];
        }

        var param = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = _publicKey,
        };
        
        if (await Helper.IsTokenValid(auth, _redis, param) && !string.IsNullOrWhiteSpace(auth))
        {
            context.Response.StatusCode = 401;
            
            const string message = "JWT is invalid";
            await context.Response.BodyWriter.WriteAsync(new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(message)));
            return;
        }
        
        await next.Invoke(context);
    }
}