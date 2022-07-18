using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using DynamicQL.Authentication.Ldap.Model;
using DynamicQL.Authentication.Ldap.Services;
using DynamicQL.Authentication.Model;
using DynamicQL.Authentication.Queries;
using DynamicQL.Authentication.Services;
using DynamicQL.Model;
using DynamicQL.Model.Types;
using DynamicQL.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicQL.Authentication.Ldap.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDynamicQLLdapAuthentication(
        this IServiceCollection serviceCollection, 
        Action<LdapAuthenticationOptions> configure)
    {
        serviceCollection.AddSingleton<LdapAuthenticationOptions>(serviceProvider =>
        {
            var options = new LdapAuthenticationOptions();
            configure.Invoke(options);
            return options;
        });

        serviceCollection.AddSingleton<AuthenticationOptions>(serviceProvider => 
            serviceProvider.GetRequiredService<LdapAuthenticationOptions>());
        
        serviceCollection.AddTransient<DynamicQLLdapAuthenticationService>();
        
        serviceCollection.AddTransient<IDynamicQLLdapAuthenticationService, DynamicQLLdapAuthenticationService>(serviceProvider => 
            serviceProvider.GetRequiredService<DynamicQLLdapAuthenticationService>());
        
        serviceCollection.AddTransient<IDynamicQLAuthenticationService, DynamicQLLdapAuthenticationService>(serviceProvider => 
            serviceProvider.GetRequiredService<DynamicQLLdapAuthenticationService>());

        var tempServiceProvider = serviceCollection.BuildServiceProvider();
        var options = tempServiceProvider.GetService<DynamicQLOptions>();

        if (options == null)
        {
            throw new Exception($"Could not resolve service of type {typeof(DynamicQLOptions).FullName}. " +
                                $"This usually happens when {nameof(AddDynamicQLLdapAuthentication)} is called " +
                                $"before AddDynamicQL");
        }
    
        if (!StaticData.TypeToGraphQLTypeMetaInfoMap.TryGetValue(typeof(Authenticate), out var fieldInfo))
        {
            fieldInfo = TypeMetaInfo.ImportAttributeInfos(typeof(Authenticate), options);
            StaticData.TypeToGraphQLTypeMetaInfoMap.Add(typeof(Authenticate), fieldInfo);
        
            serviceCollection.AddSingleton(fieldInfo.SingleQueryGraphType);
            serviceCollection.AddSingleton(fieldInfo.SingleInputGraphType);
        }

        if (!StaticData.TypeToGraphQLTypeMetaInfoMap.TryGetValue(typeof(ValuePair), out fieldInfo))
        {
            fieldInfo = TypeMetaInfo.ImportAttributeInfos(typeof(ValuePair), options);
            StaticData.TypeToGraphQLTypeMetaInfoMap.Add(typeof(ValuePair), fieldInfo);
        
            serviceCollection.AddSingleton(fieldInfo.SingleQueryGraphType);
            serviceCollection.AddSingleton(fieldInfo.SingleInputGraphType);
        }
        
        return serviceCollection;
    }
}