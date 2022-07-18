using DynamicQL.Authentication.Services;
using DynamicQL.Model.Types;
using DynamicQL.Services;
using GraphQL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DynamicQL.Authentication.Jwt.Services;

public class ValidateAuthenticationService : IValidateAuthenticationService
{
    private readonly IDynamicQLTokenService _tokenService;
    private readonly HttpContext _httpContext;
    
    public ValidateAuthenticationService(HttpContext httpContext, IDynamicQLTokenService tokenService)
    {
        _tokenService = tokenService;
        _httpContext = httpContext;
    }
    
    public async Task<ValidationResult> InvokeAsync<T>(IResolveFieldContext fieldContext)
    {
        var validationResult = new ValidationResult()
        {
            IsValidated = false,
            ErrorMessage = "Unknown error"
        };

        var authToken = _httpContext.Request.Headers.Authorization.ToString();

        if (authToken.StartsWith("Bearer "))
            authToken = authToken["Bearer ".Length..];

        validationResult.IsValidated = string.IsNullOrWhiteSpace(authToken) 
                                       || (await _tokenService.IsAuthenticationTokenValid(authToken)).Item1;
        
        validationResult.ErrorMessage = validationResult.IsValidated ? null : "The token is invalid";
        
        return validationResult;
    }
}