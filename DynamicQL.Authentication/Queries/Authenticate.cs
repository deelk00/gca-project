using DynamicQL.Attributes;
using DynamicQL.Attributes.Enums;
using DynamicQL.Authentication.Enums;
using DynamicQL.Authentication.Model;
using DynamicQL.Authentication.Services;
using DynamicQL.Extensions;
using DynamicQL.Model.Types;
using GraphQL;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace DynamicQL.Authentication.Queries;

[DynamicQL(options: QueryOptions.None, minExecutionTime: 100)]
[DynamicQLExcludeFromValidationMiddleware]
public class Authenticate
{
    public bool IsAuthenticated { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public List<ValuePair> Payload { get; set; }
    public int ExpiresIn { get; set; }

    [DynamicQLQuerySetup(SetupType.AddQueryConditional, 1, nameof(AuthenticateResolver))]
    public SetupResult ShouldAuthenticateBeAddedToQuery(AuthenticationOptions authenticationOptions) => 
        authenticationOptions.DoNotAddAuthenticationQuery 
            ? SetupResult.DoNotAddQuery 
            : SetupResult.AddQuery;
    
    [DynamicQLQuerySetup(SetupType.AddQueryConditional, 2, nameof(RefreshTokenResolver))]
    public SetupResult ShouldRefreshTokenBeAddedToQuery(AuthenticationOptions authenticationOptions) => 
        authenticationOptions.DoNotAddRefreshTokenQuery 
        || authenticationOptions.DoNotAddAuthenticationQuery 
            ? SetupResult.DoNotAddQuery 
            : SetupResult.AddQuery;
    
    [DynamicQLQuerySetup(SetupType.AddQueryConditional, 3, nameof(InvalidateTokenResolver))]
    public SetupResult ShouldInvalidateTokenBeAddedToQuery(AuthenticationOptions authenticationOptions) => 
        authenticationOptions.DoNotAddInvalidateTokenQuery
        || authenticationOptions.DoNotAddAuthenticationQuery 
            ? SetupResult.DoNotAddQuery 
            : SetupResult.AddQuery;

    [DynamicQLQuerySetup(SetupType.Setup, 4)]
    public SetupResult RenameQueries(
        AuthenticationOptions authenticationOptions, 
        TypeMetaInfo metaInfo)
    {
        metaInfo.SingleGraphName = authenticationOptions.AuthenticationQueryName;
        var authenticateResolver = metaInfo.QueryResolvers
            .First(x => x.Resolver.Name == nameof(AuthenticateResolver));
        authenticateResolver.Name = authenticationOptions.AuthenticationQueryName;
        
        var refreshTokenResolver = metaInfo.QueryResolvers
            .First(x => x.Resolver.Name == nameof(RefreshTokenResolver));
        refreshTokenResolver.Name = authenticationOptions.RefreshTokenQueryName;
        
        var invalidateTokenResolver = metaInfo.QueryResolvers
            .First(x => x.Resolver.Name == nameof(InvalidateTokenResolver));
        invalidateTokenResolver.Name = authenticationOptions.InvalidateTokenQueryName;
        
        return SetupResult.Successful;
    }

    [DynamicQLQueryResolver("authenticate", false)]
    [DynamicQLResolverArgument("username", typeof(NonNullGraphType<StringGraphType>))]
    [DynamicQLResolverArgument("password", typeof(NonNullGraphType<StringGraphType>))]
    public async Task<object?> AuthenticateResolver(IResolveFieldContext context, 
        IDynamicQLAuthenticationService authenticationService, 
        AuthenticationOptions options)
    {
        var username = context.GetArgument<string>("username");
        var password = context.GetArgument<string>("password");
        
        var authenticationResult = await authenticationService.Authenticate(username, password, options.PayloadKeys);

        var auth = new Authenticate()
        {
            IsAuthenticated = authenticationResult.IsAuthenticated,
            Token = authenticationResult.TokenResult?.Token,
            RefreshToken = authenticationResult.TokenResult?.RefreshToken,
            ExpiresIn = authenticationResult.TokenResult?.ExpiresIn ?? 0,
            Payload = authenticationResult.Payload.Select(x => new ValuePair()
            {
                Key = x.Key.ToString().ToLowerCaseCamelCase(),
                Value = x.Value?.ToString()
            }).ToList()
        };

        return auth;
    }
    
    [DynamicQLQueryResolver("refreshToken", false)]
    [DynamicQLResolverArgument("refreshToken", typeof(NonNullGraphType<StringGraphType>))]
    public async Task<object?> RefreshTokenResolver(IResolveFieldContext context, 
        IDynamicQLAuthenticationService authenticationService)
    {
        var refreshToken = context.GetArgument<string>("refreshToken");
        
        var authenticationResult = await authenticationService.RefreshToken(refreshToken);

        var auth = new Authenticate()
        {
            IsAuthenticated = authenticationResult.IsAuthenticated,
            Token = authenticationResult.TokenResult?.Token,
            RefreshToken = authenticationResult.TokenResult?.RefreshToken,
            ExpiresIn = authenticationResult.TokenResult?.ExpiresIn ?? 0,
            Payload = authenticationResult.Payload.Select(x => new ValuePair()
            {
                Key = x.Key.ToString().ToLowerCaseCamelCase(),
                Value = x.Value?.ToString()
            }).ToList()
        };

        return auth;
    }
    
    [DynamicQLQueryResolver("invalidateToken", false)]
    [DynamicQLResolverArgument("token", typeof(NonNullGraphType<StringGraphType>))]
    public async Task<object?> InvalidateTokenResolver(IResolveFieldContext context, 
        IDynamicQLAuthenticationService authenticationService)
    {
        var token = context.GetArgument<string>("token");
        
        await authenticationService.Invalidate(token);

        return new Authenticate() { IsAuthenticated = false, ExpiresIn = -1 };
    }
    
}