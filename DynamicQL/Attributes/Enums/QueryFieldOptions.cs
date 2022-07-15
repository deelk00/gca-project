namespace DynamicQL.Attributes.Enums;

[Flags]
public enum QueryFieldOptions
{
    IgnoreCase = 1,
    Contains = 2,
    StartsWith = 4,
    EndsWith = 8,
    SearchCaseSensitive = 2,
    SearchIgnoreCase = 3,
    NotEquals = 16,
    Equals = 32,
    GreaterThan = 64,
    GreaterThanOrEqual = 128,
    LessThan = 256,
    LessThanOrEqual = 512
}