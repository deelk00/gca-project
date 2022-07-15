using DynamicQL.Attributes.Enums;

namespace DynamicQL.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class DynamicQLQueryResolverAttribute : DynamicQLResolverAttribute
{
    public DynamicQLQueryResolverAttribute(string name, 
        bool returnsMultiple, 
        string? description = null, 
        string? deprecatedReason = null
        ) 
        : base(
            name,
            returnsMultiple,
            description,
            deprecatedReason,
            ResolverType.Query
            ) { }
}