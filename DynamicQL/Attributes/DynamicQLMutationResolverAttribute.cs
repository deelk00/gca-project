using DynamicQL.Attributes.Enums;

namespace DynamicQL.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class DynamicQLMutationResolverAttribute : DynamicQLResolverAttribute
{
    public DynamicQLMutationResolverAttribute(string name, 
        bool returnsMultiple, 
        string? description = null, 
        string? deprecatedReason = null) 
        : base(
            name,
            returnsMultiple,
            description,
            deprecatedReason,
            ResolverType.Mutation
        ) { }
}