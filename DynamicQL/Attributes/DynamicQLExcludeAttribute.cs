namespace DynamicQL.Attributes;

[AttributeUsage( AttributeTargets.Property)]
public class DynamicQLExcludeAttribute : Attribute
{
    public ExcludeFrom ExcludeFrom { get; set; }

    public DynamicQLExcludeAttribute(ExcludeFrom excludeFrom = ExcludeFrom.Everything)
    {
        ExcludeFrom = excludeFrom;
    }
}