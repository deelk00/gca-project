using System.Linq.Expressions;
using DynamicQL.Utility;
using GraphQL.Types;

namespace DynamicQL.Model.Types;

public sealed class InputType<T> : InputObjectGraphType<T>
{
    public InputType(DynamicQLOptions options)
    {
        var baseType = typeof(T);
        var listBaseType = typeof(List<>).GetGenericTypeDefinition();

        Name = baseType.Name + "Input";

        if (!StaticData.TypeToGraphQLTypeMetaInfoMap.TryGetValue(baseType, out var queryInfo))
        {
            queryInfo = TypeMetaInfo.ImportAttributeInfos(baseType, options);
        }
        
        foreach (var propMetaInfo in queryInfo.Properties.Values.Where(propMetaInfo => !propMetaInfo.IsExcluded))
        {
            var propGraphType = propMetaInfo.InputGraphType;
            
            if (propMetaInfo == queryInfo.IdFieldMetaInfo
                && propMetaInfo.IsNullable)
            {
                propGraphType = propGraphType.GetGenericArguments()[0];
            }

            if (propMetaInfo.BaseType.Namespace != "System")
            {
                var bType = propMetaInfo.IsEnumerable
                    ? listBaseType.MakeGenericType(propMetaInfo.BaseType)
                    : propMetaInfo.BaseType;
                
                StaticData.BaseTypeToInputTypeMap.TryGetValue(bType, out propGraphType);
            }

            if (propMetaInfo.PropertyInfo.PropertyType.IsEnum)
            {
                StaticData.EnumToGraphQLEnumTypeMap.TryGetValue(propMetaInfo.PropertyInfo.PropertyType,
                    out propGraphType);
            }
            
            if (propMetaInfo.Resolver is null)
            {
                Field(propGraphType,
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
                        type: propGraphType,
                        expression: (propMetaInfo.Resolver as Expression<Func<T, object?>>)!)
                    .Description(propMetaInfo.Description)
                    .DeprecationReason(propMetaInfo.DeprecationReason);
            }
        }
    }
}