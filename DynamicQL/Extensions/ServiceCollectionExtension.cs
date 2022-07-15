using System.ComponentModel;
using System.Reflection;
using DynamicQL.Attributes;
using DynamicQL.Model;
using DynamicQL.Model.Graphs;
using DynamicQL.Model.Schema;
using DynamicQL.Model.Types;
using DynamicQL.Services;
using DynamicQL.Utility;
using GraphQL.Types;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicQL.Extensions;

public static class ServiceCollectionExtension
{
    private static IEnumerable<Type> GetGraphQLQueryTypesFromAssembly(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract && x.IsPublic)
            .Where(x =>
                // filter out base interface
                typeof(IGraphType) != x
                && typeof(object) != x
                // filter out any non IGraphQLType types
                && x.GetCustomAttribute(typeof(DynamicQLAttribute)) != null
                && x.GetCustomAttribute(typeof(DynamicQLExcludeAttribute)) == null
            );

    }
    
    private static void CreateGraphQLTypeMetaInfos(IEnumerable<Type> types, DynamicQLOptions options)
    {
        foreach (var type in types)
        {
            var graphTypeInfo = TypeMetaInfo.ImportAttributeInfos(type, options);
            StaticData.TypeToGraphQLTypeMetaInfoMap.Add(type, graphTypeInfo);
        }
    }

    private static void RegisterTypes(IServiceCollection serviceCollection)
    {
        foreach (var fieldInfo in StaticData.TypeToGraphQLTypeMetaInfoMap.Values)
        {
            serviceCollection.AddSingleton(fieldInfo.SingleQueryGraphType);
            serviceCollection.AddSingleton(fieldInfo.SingleInputGraphType);
        }

        foreach (var enumType in StaticData.EnumToGraphQLEnumTypeMap.Values)
        {
            serviceCollection.AddSingleton(enumType);
        }
    }
    
    public static IServiceCollection AddDynamicGraphQL(this IServiceCollection serviceCollection, Action<DynamicQLOptions>? optionsFunc = null)
    {
        var options = new DynamicQLOptions();
        optionsFunc?.Invoke(options);
        
        if (options.Assemblies.Count == 0)
        {
            options.Assemblies = Assembly.GetEntryAssembly()?
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .Except(new []
                {
                    Assembly.GetExecutingAssembly()
                })
                .Concat(new []
                {
                    Assembly.GetEntryAssembly()
                })
                .Where(x => x != null)
                .ToList()!;
        }
        
        var types = options.Assemblies.SelectMany(GetGraphQLQueryTypesFromAssembly).ToList();
        types = types
            .Concat(options.Types)
            .Except(StaticData.TypeToGraphQLTypeMetaInfoMap.Keys)
            .ToList();
        
        serviceCollection.AddSingleton<DynamicQLSchema>();
        serviceCollection.AddSingleton<DynamicQLRootQuery>();
        serviceCollection.AddSingleton<DynamicQLRootMutation>();
        serviceCollection.AddSingleton<DynamicQueryLoaderService>();
        serviceCollection.AddSingleton<IDynamicQueryLoaderService, DynamicQueryLoaderService>();
        serviceCollection.AddSingleton<DynamicQLOptions>(serviceProvider =>
        {
            var opts = new DynamicQLOptions();
            optionsFunc?.Invoke(opts);
            
            return opts;
        });
        
        serviceCollection.AddSingleton<IncludeFunctionTreeGraph>();
        
        CreateGraphQLTypeMetaInfos(types, options);
        RegisterTypes(serviceCollection);
        
        return serviceCollection;
    }
}