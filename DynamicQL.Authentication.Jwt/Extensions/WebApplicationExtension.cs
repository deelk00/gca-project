using DynamicQL.Authentication.Services;
using DynamicQL.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicQL.Authentication.Jwt.Extensions;

public static class WebApplicationExtension
{
    public static WebApplication UseDynamicQLJwtValidation(this WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<DynamicQLOptions>();
        
        if(!options.DefaultValidationMiddlewareTypes.Contains(typeof(IValidateAuthenticationService)))
            options.DefaultValidationMiddlewareTypes.Add(typeof(IValidateAuthenticationService));

        return app;
    }
}