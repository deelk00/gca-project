using DynamicQL.Attributes.Enums;

namespace DynamicQL.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class DynamicQLQueryFieldAttribute : Attribute
{
    public QueryFieldOptions Options { get; }
    public string Description { get; }
    public string Name { get; }
    public string DeprecatedReason { get; }
    public object? DefaultValue { get; set; }
    public bool IsNullable { get; set; }
    public DynamicQLQueryFieldAttribute(
        QueryFieldOptions options,
        string name = null,
        object? defaultValue = null,
        string description = null,
        string deprecatedReason = null,
        bool isNullable = true)
    {
        Options = options;
        Name = name;
        DefaultValue = defaultValue;
        Description = description;
        DeprecatedReason = deprecatedReason;
        IsNullable = isNullable;
    }
}