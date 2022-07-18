using DynamicQL.Attributes;
using DynamicQL.Attributes.Enums;

namespace DynamicQL.Authentication.Model;

[DynamicQL(options: QueryOptions.None)]
public class ValuePair
{
    public string Key { get; set; }
    public string? Value { get; set; }
}