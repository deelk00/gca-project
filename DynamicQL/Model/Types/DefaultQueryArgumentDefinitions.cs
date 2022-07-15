using GraphQL.Types;

namespace DynamicQL.Model.Types;

public class DefaultQueryArgumentDefinitions
{
    public QueryArgumentDefinition Id { get; } = new()
    {
        Name = "id", Description = "Queries the database for the specified id", GraphType = typeof(ObjectGraphType)
    };

    public QueryArgumentDefinition Page { get; } = new()
    {
        Name = "page", Description = "Specifies the page of the query", GraphType = typeof(IntGraphType)
    };

    public QueryArgumentDefinition PageSize { get; } = new()
    {
        Name = "pageSize", Description = "Sets the page-size for the request", GraphType = typeof(IntGraphType)
    };

    public QueryArgumentDefinition Take { get; } = new()
    {
        Name = "take", Description = "Takes the specified values and returns them", GraphType = typeof(IntGraphType)
    };

    public QueryArgumentDefinition Skip { get; } = new()
    {
        Name = "skip", Description = "Skips the specified values and returns the items afterwards, limited by page-size", GraphType = typeof(IntGraphType)
    };

    public List<QueryArgumentDefinition> DefaultArguments = new();

    public DefaultQueryArgumentDefinitions SetSkip(string name, string? description) => Set(Skip, name, description);
    public DefaultQueryArgumentDefinitions SetTake(string name, string? description) => Set(Take, name, description);
    public DefaultQueryArgumentDefinitions SetPageSize(string name, string? description) => Set(PageSize, name, description);
    public DefaultQueryArgumentDefinitions SetPage(string name, string? description) => Set(Page, name, description);
    public DefaultQueryArgumentDefinitions SetId(string name, string? description) => Set(Id, name, description);
    
    private DefaultQueryArgumentDefinitions Set(QueryArgumentDefinition queryDef, string name, string? description)
    {
        queryDef.Name = name;
        if (description != null)
            queryDef.Description = description;
        return this;
    }
}