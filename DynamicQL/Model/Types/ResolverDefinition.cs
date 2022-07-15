using System.Reflection;
using DynamicQL.Attributes;
using DynamicQL.Extensions;
using GraphQL.Types;

namespace DynamicQL.Model.Types;

public class ResolverDefinition
{
    public bool IsEnumerableReturnType { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? DeprecatedReason { get; set; }
    public MethodInfo Resolver { get; set; }
    public QueryArguments QueryArguments { get; set; } = new QueryArguments();
    
    public ResolverDefinition Clone()
    {
        return new ResolverDefinition()
        {
            Name = Name,
            Description = Description,
            DeprecatedReason = DeprecatedReason,
            Resolver = Resolver,
            QueryArguments = new QueryArguments(QueryArguments),
        };
    }
    
    public static ResolverDefinition? ParseFromMethodInfo(MethodInfo methodInfo, DynamicQLOptions options)
    {
        var definition = new ResolverDefinition();

        var queryAttribute = methodInfo.GetCustomAttribute<DynamicQLResolverAttribute>();
        if (queryAttribute == null) return null;

        definition.IsEnumerableReturnType = queryAttribute.ReturnsMultiple;
        definition.Resolver = methodInfo;
        definition.Name = queryAttribute.Name.ToLowerCaseCamelCase();
        definition.Description = queryAttribute.Description;
        definition.DeprecatedReason = queryAttribute.DeprecatedReason;
        
        var argumentAttributes = methodInfo.GetCustomAttributes<DynamicQLResolverArgumentAttribute>();

        foreach (var argumentAttribute in argumentAttributes)
        {
            definition.QueryArguments.Add(
                new QueryArgument(argumentAttribute.GraphType)
                {
                    Name = argumentAttribute.Name,
                    Description = argumentAttribute.Description,
                    DefaultValue = argumentAttribute.DefaultValue
                }
            );
        }

        foreach (var defaultArgumentDefinition in options.DefaultQueryArguments.DefaultArguments)
        {
            definition.QueryArguments.Add(defaultArgumentDefinition.ToQueryArgument());
        }

        return definition;
    }
}