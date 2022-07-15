namespace DynamicQL.Attributes;

/// <summary>
/// Renaming a property for GraphQL is very memory intensive ~1Kb for one field
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DynamicQLPropertyAttribute : Attribute
{
    public string? Name { get; }
    public string? Description { get; }
    public string? DeprecationReason { get; }
    public NullableOptions NullableOptions { get; set; }
    
    public DynamicQLPropertyAttribute(string? name = null, 
        string? description = null, 
        string? deprecationReason = null,
        NullableOptions nullableOptions = NullableOptions.IsNotNullable)
    {
        Name = name;
        Description = description;
        DeprecationReason = deprecationReason;
        NullableOptions = nullableOptions;
    }
}