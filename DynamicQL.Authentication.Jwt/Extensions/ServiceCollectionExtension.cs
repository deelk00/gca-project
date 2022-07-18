using System.Security.Cryptography;
using DynamicQL.Authentication.Jwt.Model;
using DynamicQL.Authentication.Jwt.Services;
using DynamicQL.Authentication.Model;
using DynamicQL.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicQL.Authentication.Jwt.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDynamicQLJwtAuthentication(this IServiceCollection serviceCollection, 
        Action<JwtOptions>? configure = null)
    {
        if(serviceCollection.All(x => x.ServiceType != typeof(ValidateAuthenticationService)))
            serviceCollection.AddTransient<IValidateAuthenticationService, ValidateAuthenticationService>();
        
        serviceCollection.AddSingleton<DynamicQLJwtTokenService>();
        serviceCollection.AddSingleton<IDynamicQLJwtTokenService>(provider => 
            provider.GetRequiredService<DynamicQLJwtTokenService>());
        serviceCollection.AddSingleton<IDynamicQLTokenService>(provider =>
            provider.GetRequiredService<DynamicQLJwtTokenService>());
        serviceCollection.AddHostedService(sp => sp.GetService<DynamicQLJwtTokenService>());
        
        serviceCollection.AddSingleton<JwtOptions>(provider =>
        {
            var options = new JwtOptions();
            configure?.Invoke(options);

            if (options.PublicKey != null && options.PrivateKey != null) return options;
            
            var rsa = RSA.Create(options.KeySize);
            options.PublicKey = rsa.ExportRSAPublicKey();
            options.PrivateKey = rsa.ExportRSAPrivateKey();

            return options;
        });

        
        return serviceCollection;
    }
}