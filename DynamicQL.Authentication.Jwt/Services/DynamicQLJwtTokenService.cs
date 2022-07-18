using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using DynamicQL.Authentication.Enums;
using DynamicQL.Authentication.Jwt.Model;
using DynamicQL.Authentication.Model;
using DynamicQL.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace DynamicQL.Authentication.Jwt.Services;

public class DynamicQLJwtTokenService : IHostedService, IDynamicQLJwtTokenService
{
    private static readonly Dictionary<Guid, DateTime> InvalidatedJtis = new Dictionary<Guid, DateTime>();
    
    private readonly SymmetricSecurityKey _securityKey;
    private readonly JwtOptions _options;
    private readonly int cleanupInterval = 60 * 5 * 1000; // run cleanup every 5 min
    private readonly CancellationTokenSource StoppingToken = new CancellationTokenSource();
    
    public DynamicQLJwtTokenService(JwtOptions options)
    {
        _options = options;
        var rsaKey = RSA.Create();
        rsaKey.ImportRSAPrivateKey(options.PrivateKey, out _);
        var pKey = rsaKey.ExportRSAPublicKey();
        
        // verify that public and private key are connected
        for (var i = 0; i < options.PublicKey?.Length && i < pKey.Length; i++)
        {
            if (pKey[i] != options.PublicKey[i])
                throw new Exception("Private and Public key are not connected to each other");
        }

        _securityKey = new SymmetricSecurityKey(rsaKey.ExportRSAPrivateKey());
    }
    
    public Task<TokenResult> CreateToken(Dictionary<PayloadKey, object?> payload)
    {
        if (!payload.TryGetValue(PayloadKey.Jti, out var jti))
        {
            jti = Guid.NewGuid();
            payload.Add(PayloadKey.Jti, jti);
        }
        
        var tokenResult = new TokenResult();
        var refreshTokenPayload = new Dictionary<PayloadKey, object?>()
        {
            { PayloadKey.IsRefreshToken, true },
            { PayloadKey.Jti, jti }
        };

        if (payload.TryGetValue(PayloadKey.UserId, out var userId))
        {
            refreshTokenPayload.Add(PayloadKey.UserId, userId);
        }
        
        tokenResult.Token = CreateJwt(_options.TokenExpirationTime, payload);
        tokenResult.RefreshToken = CreateJwt(_options.RefreshTokenExpirationTime, refreshTokenPayload);
        tokenResult.ExpiresIn = _options.TokenExpirationTime;

        return Task.FromResult(tokenResult);
    }

    private string CreateJwt(int expirationTime, IDictionary<PayloadKey, object?>? payload = null)
    { 
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddSeconds(expirationTime),
            NotBefore = DateTime.UtcNow.AddSeconds(_options.ClockSkewInSeconds),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            SigningCredentials = new SigningCredentials(_securityKey, _options.SecurityAlgorithm),
            Claims = payload?.ToDictionary(x => 
                Enum.GetName(x.Key)!.ToLowerCaseCamelCase()!, x => x.Value!)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private readonly string[] _payloadKeyNames = Enum.GetNames(typeof(PayloadKey));
    public Task<Dictionary<PayloadKey, object?>?> ParseToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var claims = jwtSecurityToken.Claims.ToList();

            var payload = new Dictionary<PayloadKey, object?>();
            
            foreach (var claim in claims)
            {
                if (_payloadKeyNames.FirstOrDefault(x => x.ToLowerCaseCamelCase() == claim.Type) 
                    is not { } name) continue;
                
                var key = Enum.Parse<PayloadKey>(name);
                payload.Add(key, claim.Value);
            }

            return Task.FromResult(payload)!;
        }
        catch(Exception ex)
        {
            return Task.FromResult<Dictionary<PayloadKey, object?>?>(null);
        }
    }

    public async Task<(bool, Dictionary<PayloadKey, object?>?)> IsAuthenticationTokenValid(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            tokenHandler.ValidateToken(token, new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _options.Issuer,
                ValidAudience = _options.Audience,
                IssuerSigningKey = _securityKey,
            }, out var validatedToken);
            
            var payload = await ParseToken(token);

            if (validatedToken.ValidTo < DateTime.UtcNow)
            {
                return (false, payload);
            }
            
            if (payload?.TryGetValue(PayloadKey.Jti, out var jtiObj) == true
                && Guid.TryParse(jtiObj?.ToString(), out var jti)
                && InvalidatedJtis.ContainsKey(jti))
            {
                return (false, payload);
            }
            
            var res = payload?.TryGetValue(PayloadKey.IsRefreshToken, out var v) == true 
                ? (v is not true, payload) 
                : (true, payload);
            return res;
        }
        catch(Exception ex)
        {
            return (false, null);
        }
    }

    public async Task<(bool, Dictionary<PayloadKey, object?>?)> IsRefreshTokenValid(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            tokenHandler.ValidateToken(token, new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _options.Issuer,
                ValidAudience = _options.Audience,
                IssuerSigningKey = _securityKey,
            }, out var validatedToken);

            var payload = await ParseToken(token);

            if (validatedToken.ValidTo < DateTime.Now)
            {
                return (false, payload);
            }
            
            if (payload?.TryGetValue(PayloadKey.Jti, out var jtiObj) == true
                && Guid.TryParse(jtiObj?.ToString(), out var jti)
                && InvalidatedJtis.ContainsKey(jti))
            {
                return (false, payload);
            }
            
            var res = payload?.TryGetValue(PayloadKey.IsRefreshToken, out var v) == true 
                ? (bool.TryParse(v?.ToString(), out var b) && b, payload) 
                : (false, null);
            return res;
        }
        catch(Exception ex)
        {
            return (false, null);
        }
    }

    public async Task InvalidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            tokenHandler.ValidateToken(token, new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _options.Issuer,
                ValidAudience = _options.Audience,
                IssuerSigningKey = _securityKey,
            }, out var validatedToken);

            var payload = await ParseToken(token);

            var jti = Guid.Parse(payload![PayloadKey.Jti]?.ToString()!);
            InvalidatedJtis.Add(jti, validatedToken.ValidTo);
        }
        catch
        {
            // ignored (token is already invalid)
        }
    }

    // cleanup tokens
    protected async Task CleanupTokens()
    {
        do
        {
            try
            {
                var invalidJtis = InvalidatedJtis
                    .Where(x => x.Value < DateTime.Now)
                    .ToList();

                foreach (var (jti, invalidationTime) in invalidJtis)
                {
                    InvalidatedJtis.Remove(jti);
                }
            
                await Task.Delay(cleanupInterval, StoppingToken.Token);
            }
            catch
            {
                // ignored
            }
        } while (!StoppingToken.IsCancellationRequested);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Run(CleanupTokens, StoppingToken.Token);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        StoppingToken.Cancel();
        return Task.CompletedTask;
    }
}