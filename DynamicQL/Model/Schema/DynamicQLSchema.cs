using DynamicQL.Utility;

namespace DynamicQL.Model.Schema;

public class DynamicQLSchema : GraphQL.Types.Schema
{
    public DynamicQLSchema(IServiceProvider provider, DynamicQLRootQuery query, DynamicQLRootMutation mutation)
        : base(provider)
    {
        Query = query;
        Mutation = mutation;

        RegisterTypes(StaticData.TypeToGraphQLTypeMetaInfoMap.Values.Select(x => x.SingleQueryGraphType).ToArray());
        RegisterTypes(StaticData.TypeToGraphQLTypeMetaInfoMap.Values.Select(x => x.MultiQueryGraphType).ToArray());
    }
}