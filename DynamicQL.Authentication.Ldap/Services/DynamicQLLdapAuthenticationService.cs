using System.Security.Cryptography.X509Certificates;
using System.Text;
using DynamicQL.Authentication.Enums;
using DynamicQL.Authentication.Ldap.Model;
using DynamicQL.Authentication.Ldap.Utility;
using DynamicQL.Authentication.Model;
using DynamicQL.Authentication.Services;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;
using Novell.Directory.Ldap;

namespace DynamicQL.Authentication.Ldap.Services;

public class DynamicQLLdapAuthenticationService : IDynamicQLLdapAuthenticationService
{
    private static readonly Dictionary<PayloadKey, string> PayloadKeyToLdapPropertyMap = new()
    {
        { PayloadKey.Email, LdapProperties.EMAILADDRESS },
        { PayloadKey.UserName, LdapProperties.USERPRINCIPALNAME },
        { PayloadKey.DisplayName, LdapProperties.DISPLAYNAME },
        { PayloadKey.FirstName, LdapProperties.FIRSTNAME },
        { PayloadKey.LastName, LdapProperties.LASTNAME },
        { PayloadKey.MiddleName, LdapProperties.MIDDLENAME },
    };
    private readonly LdapAuthenticationOptions _authenticationOptions;
    private readonly LdapConnection _connection;
    private readonly IDynamicQLTokenService _tokenService;
    private readonly IDynamicQLPayloadProcessorService? _payloadProcessorService;
    
    public DynamicQLLdapAuthenticationService(LdapAuthenticationOptions authenticationOptions, 
        IDynamicQLTokenService tokenService, 
        IServiceProvider provider)
    {
        _authenticationOptions = authenticationOptions;
        _tokenService = tokenService;
        _payloadProcessorService = provider.GetService<IDynamicQLPayloadProcessorService>();
        _connection = new LdapConnection();

        if (authenticationOptions.Protocol != LdapProtocol.LdapSecure) return;
        
        _connection.SecureSocketLayer = authenticationOptions.Protocol == LdapProtocol.LdapSecure;
        _connection.UserDefinedServerCertValidationDelegate += CheckLdapCertificate;
    }

    private bool CheckLdapCertificate(object sender, 
        X509Certificate certificate, 
        X509Chain chain, 
        System.Net.Security.SslPolicyErrors sslPolicyErrors)
    {
        if(sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
        {
            return true;
        }

        if (_authenticationOptions.Certificate == null)
            return false;
        
        var chain0 = new X509Chain();
        chain0.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
        
        chain0.ChainPolicy.ExtraStore.Add(new X509Certificate2(_authenticationOptions.Certificate));
        chain0.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
        
        var isValid = chain0.Build((X509Certificate2)certificate);

        return isValid;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="payloadKeys">Key -> Ldap property to resolve, Value -> payload key</param>
    /// <returns></returns>
    public async Task<AuthenticationResult> Authenticate(string username, string password, List<PayloadKey> payloadKeys)
    {
        var domain = _authenticationOptions.Domain;
        var baseDn = _authenticationOptions.BaseDn;
        
        var userDn = $"{username}@{domain}";
        if (username.EndsWith($"@{domain}")) userDn = username;

        if(payloadKeys.Count == 0)
            payloadKeys.Add(PayloadKey.UserName);
        
        var authResult = new AuthenticationResult()
        {
            IsAuthenticated = false,
            Payload = new Dictionary<PayloadKey, object?>()
        };
        
        try
        {
            _connection.Connect(domain, _authenticationOptions.Port);
            _connection.Bind(userDn, password);
            
            if (_connection.Bound)
            {
                authResult.IsAuthenticated = true;
                
                var validLdapQueries = new StringBuilder("(|");
                validLdapQueries.Append($"(userPrincipalName={userDn})");
                validLdapQueries.Append($"(sAMAccountName={username})");
                validLdapQueries.Append($"(cn={username})");
                validLdapQueries.Append(")");

                var search = _connection.Search(
                    baseDn,
                    LdapConnection.ScopeSub,
                    validLdapQueries.ToString(),
                    PayloadKeyToLdapPropertyMap.Where(x => 
                        payloadKeys.Contains(x.Key))
                        .Select(x => x.Value)
                        .ToArray(),
                    false
                );

                var result = search.Next();

                if (_connection.Connected) _connection.Disconnect();

                var attributes = result.GetAttributeSet();
                
                foreach (var att in attributes) 
                    authResult.Payload.Add(
                        PayloadKeyToLdapPropertyMap.First(x => x.Value == att.Name).Key, 
                        att.StringValue
                        );

                if (_payloadProcessorService != null)
                {
                    authResult.Payload = await _payloadProcessorService.ProcessPayload(authResult.Payload);
                }
                
                authResult.TokenResult = await _tokenService.CreateToken(authResult.Payload);
            }

            if (_connection.Connected) _connection.Disconnect();
        }
        //only catch invalid credentials
        catch (LdapException ex)
        {
        }

        return authResult;
    }

    public async Task<AuthenticationResult> RefreshToken(string token)
    {
        var authResult = new AuthenticationResult()
        {
            IsAuthenticated = false,
            Payload = new Dictionary<PayloadKey, object?>(),
            TokenResult = null
        };

        if (await _tokenService.IsRefreshTokenValid(token) is not
            {
                Item1: true,
                Item2: var payload
            })
        {
            return authResult;
        }

        authResult.IsAuthenticated = true;
        authResult.Payload = payload;
        
        if (!payload.TryGetValue(PayloadKey.UserId, out var userId)
            || userId == null)
        {
            throw new ExecutionError("Refresh token does not contain a valid user id");
        }

        if (_payloadProcessorService != null)
        {
            payload = await _payloadProcessorService.GetPayload(userId);
            authResult.Payload = payload;
        }

        authResult.TokenResult = await _tokenService.CreateToken(payload);

        return authResult;
    }

    public Task Invalidate(string token) => _tokenService.InvalidateToken(token);
}