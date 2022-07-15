using System.Collections;
using DynamicQL.Extensions;
using DynamicQL.Model.Types;
using DynamicQL.Utility;
using GraphQL.Types;

namespace DynamicQL.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class DynamicQLResolverArgumentAttribute : Attribute
{
    public string Name { get; }
    public string? Description { get; set; }
    public Type GraphType { get; }
    public object? DefaultValue { get; set; }
    
    public DynamicQLResolverArgumentAttribute(string name, Type graphType, string? description = null, object? defaultValue = null)
    {
        Name = name;
        Description = description;
        DefaultValue = defaultValue;
        if (graphType.IsAssignableTo(typeof(IGraphType)))
        {
            GraphType = graphType;
        }
        else
        {
            var argumentBaseType = graphType.GetEnumerableType() ?? graphType;

            if (!StaticData.BaseTypeToQueryTypeMap.TryGetValue(argumentBaseType, out var qlType))
            {
                qlType = typeof(InputType<>).GetGenericTypeDefinition().MakeGenericType(argumentBaseType);
            }
            
            if (graphType.IsAssignableTo(typeof(IEnumerable)))
            {
                qlType = typeof(ListGraphType<>).GetGenericTypeDefinition().MakeGenericType(qlType);
            }

            GraphType = qlType;
        }
    }
}