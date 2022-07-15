using System.Linq.Expressions;
using DynamicQL.Extensions;
using DynamicQL.Utility;
using GraphQL.Types;

namespace DynamicQL.Model.Types;

public sealed class QueryType<T> : ObjectGraphType<T>
    where T : class
{
    public QueryType(DynamicQLOptions options)
    {
        var baseType = typeof(T);

        Name = baseType.Name;

        if (!StaticData.TypeToGraphQLTypeMetaInfoMap.TryGetValue(baseType, out var queryInfo))
        {
            queryInfo = TypeMetaInfo.ImportAttributeInfos(baseType, options);
        }
        
        foreach (var propMetaInfo in queryInfo.Properties.Values.Where(propMetaInfo => !propMetaInfo.IsExcluded))
        {
            if (propMetaInfo.Resolver is null)
            {
                Field(propMetaInfo.OutputGraphType,
                    propMetaInfo.Name,
                    propMetaInfo.Description,
                    null,
                    null,
                    propMetaInfo.DeprecationReason
                );
            }
            else 
            {
                Field(propMetaInfo.Name,
                        type: propMetaInfo.OutputGraphType,
                        expression: (propMetaInfo.Resolver as Expression<Func<T, object?>>)!)
                    .Description(propMetaInfo.Description)
                    .DeprecationReason(propMetaInfo.DeprecationReason);
            }
        }
        
    }
}