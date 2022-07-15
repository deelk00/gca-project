using GraphQL.Types;

namespace DynamicQL.Model.Types;

public class QueryArgumentDefinition
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Type GraphType { get; set; } = null!;
    public object? DefaultValue { get; set; } = null!;

    public QueryArgument ToQueryArgument()
    {
        return new QueryArgument(GraphType)
        {
            Name = Name,
            Description = Description,
            DefaultValue = DefaultValue,
        };
    }
}