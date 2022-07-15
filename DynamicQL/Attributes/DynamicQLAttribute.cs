using DynamicQL.Attributes.Enums;
using Humanizer;

namespace DynamicQL.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class DynamicQLAttribute : Attribute
{
    public string? SingleName { get; }
    public string? PluralName { get; }
    public int? PageSize { get; }
    public int MinimumExecutionTime { get; }
    public QueryOptions Options { get; } = QueryOptions.All;
    
    public DynamicQLAttribute(int pageSize)
    {
        SingleName = null;
        PluralName = null;
        PageSize = pageSize < 1 ? null : pageSize;
    }
    
    public DynamicQLAttribute(string singleName, int pageSize)
    {
        SingleName = singleName;
        PluralName = singleName.Pluralize();
        PageSize = pageSize < 1 ? null : pageSize;
    }
    
    public DynamicQLAttribute(string? singleName = null, 
        string? pluralName = null, 
        int pageSize = -1, 
        QueryOptions options = QueryOptions.All,
        int minExecutionTime = 0)
    {
        SingleName = singleName;
        PluralName = pluralName;
        PageSize = pageSize < 1 ? null : pageSize;
        Options = options;
        MinimumExecutionTime = minExecutionTime;
    }
}