using DynamicQL.Attributes.Enums;

namespace DynamicQL.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class DynamicQLResolverAttribute : Attribute
{
    public string Name { get; set; }
    public bool ReturnsMultiple { get; set; }
    public string? Description { get; set; }
    public string? DeprecatedReason { get; set; }
    public ResolverType ResolverType { get; set; }
    
    public DynamicQLResolverAttribute(string name, 
        bool returnsMultiple, 
        string? description, 
        string? deprecatedReason,
        ResolverType resolverType
        )
    {
        Name = name;
        ReturnsMultiple = returnsMultiple;
        Description = description;
        DeprecatedReason = deprecatedReason;
        ResolverType = resolverType;
    }
}