using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using DynamicQL.Attributes;
using DynamicQL.Attributes.Enums;
using DynamicQL.Extensions;
using DynamicQL.Utility;
using GraphQL;
using GraphQL.Types;
using Humanizer;
using ResolverType = DynamicQL.Attributes.Enums.ResolverType;

namespace DynamicQL.Model.Types;

public class TypeMetaInfo
{
    public string SingleQueryName => $"{Options.PrependQuery}{SingleGraphName.ToUpperCaseCamelCase()}".ToLowerCaseCamelCase();
    public string MultiQueryName => $"{Options.PrependQuery}{MultiGraphName.ToUpperCaseCamelCase()}".ToLowerCaseCamelCase();
    public string SingleCreateName => $"{Options.PrependCreate}{SingleGraphName.ToUpperCaseCamelCase()}".ToLowerCaseCamelCase();
    public string MultiCreateName => $"{Options.PrependCreate}{MultiGraphName.ToUpperCaseCamelCase()}".ToLowerCaseCamelCase();
    public string SingleUpdateName => $"{Options.PrependUpdate}{SingleGraphName.ToUpperCaseCamelCase()}".ToLowerCaseCamelCase();
    public string MultiUpdateName => $"{Options.PrependUpdate}{MultiGraphName.ToUpperCaseCamelCase()}".ToLowerCaseCamelCase();
    public string SingleDeleteName => $"{Options.PrependDelete}{SingleGraphName.ToUpperCaseCamelCase()}".ToLowerCaseCamelCase();
    public string MultiDeleteName => $"{Options.PrependDelete}{MultiGraphName.ToUpperCaseCamelCase()}".ToLowerCaseCamelCase();
    
    public int MinimumExecutionTime { get; set; }
    public string SingleGraphName { get; set; }
    public string MultiGraphName { get; set; }
    public int? PageSize { get; set; }
    public DynamicQLOptions Options;
    public List<ResolverDefinition> QueryResolvers { get; set; } = new();
    public List<ResolverDefinition> MutationResolvers { get; set; } = new();
    public QueryOptions QueryOptions { get; set; }
    public Type BaseType { get; set; }
    public Type SingleQueryGraphType { get; set; }
    public Type MultiQueryGraphType { get; set; }
    public Type SingleInputGraphType { get; set; }
    public Type MultiInputGraphType { get; set; }
    public List<Type> ValidationMiddlewares { get; set; }
    public List<Type?> ExcludedValidationMiddlewares { get; set; }
    public Dictionary<string, QueryField> QueryFields { get; set; } = new Dictionary<string, QueryField>();
    public List<object> OrderByFields { get; set; } = new ();
    public PropertyMetaInfo? IdFieldMetaInfo { get; set; }
    
    /// <summary>
    /// string -> PropertyName
    /// bool -> IsSingleType
    /// </summary>
    public Dictionary<string, PropertyMetaInfo> Properties { get; set; } = new ();
    public Dictionary<string, PropertyMetaInfo> SubQueries { get; set; } = new ();

    public override string ToString()
    {
        return $"{BaseType.Name}, {SingleQueryGraphType.Name}, {MultiQueryGraphType.Name}";
    }

    public TypeMetaInfo Clone()
    {
        var newInfo = new TypeMetaInfo()
        {
            BaseType = BaseType,
            MultiQueryGraphType = MultiQueryGraphType,
            OrderByFields = OrderByFields.Select(x => ((dynamic)x).Clone()).ToList(),
            PageSize = PageSize,
            QueryOptions = QueryOptions,
            QueryResolvers = QueryResolvers.Select(x => x.Clone()).ToList(),
            SingleQueryGraphType = SingleQueryGraphType,
            SingleGraphName = SingleGraphName,
            MultiGraphName = MultiGraphName
        };

        newInfo.Properties = Properties.ToDictionary(
            x => x.Key.Clone().ToString()!,
            x => x.Value.Clone(newInfo)
        );
            
        newInfo.QueryFields = QueryFields.ToDictionary(
            x => x.Key.Clone().ToString()!,
            x => x.Value.Clone(
                newInfo, 
                newInfo.Properties[x.Key]
                )
        );

        newInfo.IdFieldMetaInfo = IdFieldMetaInfo != null
            ? Properties[IdFieldMetaInfo.PropertyInfo.PropertyType.Name]
            : null;
        
        newInfo.SubQueries = SubQueries.ToDictionary(
            x => x.Key.Clone().ToString()!,
            x => newInfo.Properties[x.Key]
        );
        
        return newInfo;
    }

    private PropertyMetaInfo? GetIdFieldMetaInfo(string singleName, DynamicQLOptions options)
    {
        return  this.Properties?
                          .Select(x => x.Value)
                          .FirstOrDefault(x =>
                        x.PropertyInfo.GetCustomAttribute<KeyAttribute>() != null
                        )
                    ?? Properties.FirstOrDefault(
                            x => x.Key == "id"
                        ).Value
                    ?? Properties?
                        .FirstOrDefault(x => 
                        x.Key == (x.Value.PropertyInfo.Name + "Id").ToLowerCaseCamelCase()
                        || x.Key == ("Id" + x.Value.PropertyInfo.Name).ToLowerCaseCamelCase())
                        .Value;
    }
    
    public Expression<Func<TEntity, bool>>? BuildIdEqualsExpression<TEntity>(object id)
        where TEntity : class
    {
        if (IdFieldMetaInfo == null) return null;

        var param = Expression.Parameter(typeof(TEntity), "x");
        var constant = Expression.Constant(id);
        var accessProperty = Expression.PropertyOrField(param, IdFieldMetaInfo.PropertyInfo.Name); 
        var expression = Expression.Equal(accessProperty, constant);
        
        return Expression.Lambda<Func<TEntity, bool>>(expression, param);
    }
    
    private static Type GetBaseType(Type type) => !type.IsAssignableTo(typeof(IEnumerable)) ? type : type.GetGenericArguments()[0];
    private int? GetPageSize() => BaseType.GetCustomAttribute<DynamicQLAttribute>()?.PageSize;

    private List<ResolverDefinition> GetMutationResolvers(DynamicQLOptions options)
    {
        
        var generalQueryDescription = BaseType.GetCustomAttribute<DynamicQLPropertyAttribute>()?.Description
                                      ?? BaseType.GetCustomAttribute<GraphQLMetadataAttribute>()?.Description
                                      ?? BaseType.GetCustomAttribute<DescriptionAttribute>()?.Description 
                                      ?? BaseType.Description();

        var generalDeprecationReason = BaseType.GetCustomAttribute<DynamicQLPropertyAttribute>()?.DeprecationReason
                                       ?? BaseType.GetCustomAttribute<GraphQLMetadataAttribute>()?.DeprecationReason;
        
        var methods = BaseType.GetMethods();

        List<ResolverDefinition> resolvers = methods
            .Where(x => !x.IsSpecialName)
            .Where(x => x.GetCustomAttribute<DynamicQLResolverAttribute>()?.ResolverType == ResolverType.Mutation)
            .Select(x => ResolverDefinition.ParseFromMethodInfo(x, Options))
            .Where(x => x != null)
            .ToList()!;
        
        
        if (QueryOptions.HasFlag(QueryOptions.SingleCreate)
           && resolvers.All(x => x.Name != SingleCreateName))
       {
           var definition = new ResolverDefinition
           {
               Name = SingleCreateName,
               Description = generalQueryDescription + " (create mutation)",
               DeprecatedReason = generalDeprecationReason,
               Resolver = StaticData.SingleCreateResolverMethodInfo.MakeGenericMethod(BaseType),
               IsEnumerableReturnType = false
           };

           definition.QueryArguments.Add(new QueryArgument(SingleInputGraphType) { Name = SingleGraphName });
            
           foreach (var defaultArgumentDefinition in options.DefaultQueryArguments.DefaultArguments)
           {
               definition.QueryArguments.Add(defaultArgumentDefinition.ToQueryArgument());
           }
           
           resolvers.Add(definition);
       }
       
       if (QueryOptions.HasFlag(QueryOptions.MultiCreate)
           && resolvers.All(x => x.Name != MultiCreateName))
       {
           var definition = new ResolverDefinition
           {
               Name = MultiCreateName,
               Description = generalQueryDescription + " (create mutation)",
               DeprecatedReason = generalDeprecationReason,
               Resolver = StaticData.MultiCreateResolverMethodInfo.MakeGenericMethod(BaseType),
               IsEnumerableReturnType = true
           };

           definition.QueryArguments.Add(new QueryArgument(MultiInputGraphType) { Name = MultiGraphName });
           
           foreach (var defaultArgumentDefinition in options.DefaultQueryArguments.DefaultArguments)
           {
               definition.QueryArguments.Add(defaultArgumentDefinition.ToQueryArgument());
           }
           
           resolvers.Add(definition);
       }
       
       if (QueryOptions.HasFlag(QueryOptions.SingleUpdate)
           && resolvers.All(x => x.Name != SingleUpdateName))
       {
           var definition = new ResolverDefinition
           {
               Name = SingleUpdateName,
               Description = generalQueryDescription + " (update mutation)",
               DeprecatedReason = generalDeprecationReason,
               Resolver = StaticData.SingleUpdateResolverMethodInfo.MakeGenericMethod(BaseType),
               IsEnumerableReturnType = false
           };

           definition.QueryArguments.Add(new QueryArgument(SingleInputGraphType) { Name = SingleGraphName });

           foreach (var defaultArgumentDefinition in options.DefaultQueryArguments.DefaultArguments)
           {
               definition.QueryArguments.Add(defaultArgumentDefinition.ToQueryArgument());
           }
           
           resolvers.Add(definition);
       }
       
       if (QueryOptions.HasFlag(QueryOptions.MultiUpdate)
           && resolvers.All(x => x.Name != MultiUpdateName))
       {
           var definition = new ResolverDefinition
           {
               Name = MultiUpdateName,
               Description = generalQueryDescription + " (update mutation)",
               DeprecatedReason = generalDeprecationReason,
               Resolver = StaticData.MultiUpdateResolverMethodInfo.MakeGenericMethod(BaseType),
               IsEnumerableReturnType = true
           };
           
           foreach (var defaultArgumentDefinition in options.DefaultQueryArguments.DefaultArguments)
           {
               definition.QueryArguments.Add(defaultArgumentDefinition.ToQueryArgument());
           }
           
           definition.QueryArguments.Add(new QueryArgument(MultiInputGraphType) { Name = MultiGraphName });
            
           resolvers.Add(definition);
       }
       
       if (QueryOptions.HasFlag(QueryOptions.SingleDelete)
           && resolvers.All(x => x.Name != SingleDeleteName))
       {
           var definition = new ResolverDefinition
           {
               Name = SingleDeleteName,
               Description = generalQueryDescription + " (delete mutation)",
               DeprecatedReason = generalDeprecationReason,
               IsEnumerableReturnType = false
           };

           if (IdFieldMetaInfo != null)
           {
               definition.Resolver = StaticData.SingleDeleteByIdResolverMethodInfo.MakeGenericMethod(BaseType);
               definition.QueryArguments.Add(new QueryArgument(IdFieldMetaInfo.InputGraphType) { Name = Options.DefaultQueryArguments.Id.Name });
           }
           else
           {
               definition.Resolver = StaticData.SingleDeleteByEntityResolverMethodInfo.MakeGenericMethod(BaseType);
               definition.QueryArguments.Add(new QueryArgument(SingleInputGraphType) { Name = SingleDeleteName });
           }
           
           foreach (var defaultArgumentDefinition in options.DefaultQueryArguments.DefaultArguments)
           {
               definition.QueryArguments.Add(defaultArgumentDefinition.ToQueryArgument());
           }
           
           resolvers.Add(definition);
       }
       
       if (QueryOptions.HasFlag(QueryOptions.MultiDelete)
           && resolvers.All(x => x.Name != MultiDeleteName))
       {
           var definition = new ResolverDefinition
           {
               Name = MultiDeleteName,
               Description = generalQueryDescription + " (delete mutation)",
               DeprecatedReason = generalDeprecationReason,
               IsEnumerableReturnType = true
           };

           if (IdFieldMetaInfo != null)
           {
               definition.QueryArguments.Add(
                   new QueryArgument(MakeGenericType(typeof(ListGraphType<>), IdFieldMetaInfo.InputGraphType))
                   {
                       Name = Options.DefaultQueryArguments.Id.Name
                   });
               definition.Resolver = StaticData.MultiDeleteByIdResolverMethodInfo.MakeGenericMethod(BaseType);
           }
           else
           {
               definition.QueryArguments.Add(new QueryArgument(MultiInputGraphType) { Name = MultiGraphName });
               definition.Resolver = StaticData.MultiDeleteByEntityResolverMethodInfo.MakeGenericMethod(BaseType);
           }
           
           foreach (var defaultArgumentDefinition in options.DefaultQueryArguments.DefaultArguments)
           {
               definition.QueryArguments.Add(defaultArgumentDefinition.ToQueryArgument());
           }
           
           resolvers.Add(definition);
       }
       
       return resolvers;
    }
    private List<ResolverDefinition> GetQueryResolvers(DynamicQLOptions options)
    {
        SingleGraphName = BaseType.GetCustomAttribute<DynamicQLAttribute>()?.SingleName
                              ?? BaseType.GetCustomAttribute<GraphQLMetadataAttribute>()?.Name
                              ?? BaseType.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
                              ?? BaseType.Name;

        MultiGraphName = BaseType.GetCustomAttribute<DynamicQLAttribute>()?.PluralName
                             ?? BaseType.GetCustomAttribute<GraphQLMetadataAttribute>()?.Name.Pluralize()
                             ?? BaseType.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName.Pluralize()
                             ?? BaseType.Name.Pluralize();

        SingleGraphName = SingleGraphName.ToLowerCaseCamelCase();
        MultiGraphName = MultiGraphName.ToLowerCaseCamelCase();

        var methods = BaseType.GetMethods();

        List<ResolverDefinition> resolvers = methods
            .Where(x => !x.IsSpecialName)
            .Where(x => x.GetCustomAttribute<DynamicQLResolverAttribute>()?.ResolverType == ResolverType.Query)
            .Select(x => ResolverDefinition.ParseFromMethodInfo(x, options))
            .Where(x => x != null)
            .ToList()!;

        var generalQueryDescription = BaseType.GetCustomAttribute<DynamicQLPropertyAttribute>()?.Description
                                     ?? BaseType.GetCustomAttribute<GraphQLMetadataAttribute>()?.Description
                                     ?? BaseType.GetCustomAttribute<DescriptionAttribute>()?.Description 
                                     ?? BaseType.Description();

        var generalDeprecationReason = BaseType.GetCustomAttribute<DynamicQLPropertyAttribute>()?.DeprecationReason
                                      ?? BaseType.GetCustomAttribute<GraphQLMetadataAttribute>()?.DeprecationReason;

        IdFieldMetaInfo = GetIdFieldMetaInfo(SingleGraphName, options);
        if (QueryOptions.HasFlag(QueryOptions.SingleQuery)
           && resolvers.All(x => x.Name != SingleQueryName))
        {
            var fieldMetaInfo = IdFieldMetaInfo;
            if (fieldMetaInfo != null)
            {
                var graphType = typeof(NonNullGraphType<>)
                   .GetGenericTypeDefinition()
                   .MakeGenericType(fieldMetaInfo.InputGraphType);

                var definition = new ResolverDefinition
                {
                    Name = SingleQueryName,
                    Description = generalQueryDescription,
                    DeprecatedReason = generalDeprecationReason,
                    Resolver = StaticData.SingleQueryResolverMethodInfo.MakeGenericMethod(BaseType),
                    IsEnumerableReturnType = false
                };
                definition.QueryArguments.Add(
                    new QueryArgument(graphType)
                    {
                        Name = options.DefaultQueryArguments.Id.Name,
                        Description = options.DefaultQueryArguments.Id.Description,
                    });

                foreach (var defaultArgumentDefinition in options.DefaultQueryArguments.DefaultArguments)
                {
                    definition.QueryArguments.Add(defaultArgumentDefinition.ToQueryArgument());
                }

                resolvers.Add(definition);
            }
        }

        if (QueryOptions.HasFlag(QueryOptions.MultiQuery)
           && resolvers.All(x => x.Name != MultiQueryName))
        {
           var definition = new ResolverDefinition
           {
               Name = MultiQueryName,
               Description = generalQueryDescription,
               DeprecatedReason = generalDeprecationReason,
               Resolver = StaticData.MultiQueryResolverMethodInfo.MakeGenericMethod(BaseType),
               IsEnumerableReturnType = true
           };

           definition.QueryArguments.Add(options.DefaultQueryArguments.Page.ToQueryArgument());
           definition.QueryArguments.Add(options.DefaultQueryArguments.PageSize.ToQueryArgument());
           definition.QueryArguments.Add(options.DefaultQueryArguments.Take.ToQueryArgument());
           definition.QueryArguments.Add(options.DefaultQueryArguments.Skip.ToQueryArgument());
           
           foreach (var defaultArgumentDefinition in options.DefaultQueryArguments.DefaultArguments)
           {
               definition.QueryArguments.Add(defaultArgumentDefinition.ToQueryArgument());
           }
           
           foreach (var (name, queryField) in QueryFields)
           {
               var propType = queryField.PropertyMetaInfo.InputGraphType;
               
               propType = queryField.IsNullable switch
               {
                   true when propType.IsAssignableTo(typeof(NonNullGraphType)) => propType.GenericTypeArguments[0],
                   false when propType.IsAssignableTo(typeof(NonNullGraphType)) => typeof(NonNullGraphType<>)
                       .GetGenericTypeDefinition()
                       .MakeGenericType(propType),
                   _ => propType
               };

               definition.QueryArguments.Add(new QueryArgument(propType)
               {
                   Name = name,
                   Description = queryField.Description,
                   DefaultValue = queryField.DefaultValue,
                   Metadata = { { "DeprecatedReason", queryField.DeprecatedReason} }
               });
           }
           resolvers.Add(definition);
        }

        return resolvers;
    }

    private static Type MakeGenericType(Type type, params Type[] genericArg) => type.IsGenericTypeDefinition
        ? type.MakeGenericType(genericArg)
        : type.GetGenericTypeDefinition().MakeGenericType(genericArg);

    private Type GetSingleGraphType() => MakeGenericType(typeof(QueryType<>), BaseType);
    private Type GetMultiGraphType() => MakeGenericType(typeof(ListGraphType<>), SingleQueryGraphType);
    private Type GetSingleInputGraphType() => MakeGenericType(typeof(InputType<>), BaseType);
    private Type GetMultiInputGraphType() => MakeGenericType(typeof(ListGraphType<>), SingleInputGraphType);

    private QueryOptions GetDynamicQLAttribute() => BaseType.GetCustomAttribute<DynamicQLAttribute>()?.Options 
                                                    ?? QueryOptions.All;

    private Dictionary<string, PropertyMetaInfo> GetProperties()
    {
        var props = BaseType.GetProperties()
            .Where(x => x.GetCustomAttribute<DynamicQLExcludeAttribute>() == null)
            .ToList();
        
        var propBaseInfoType = new PropertyMetaInfo();

        var propList = new Dictionary<string, PropertyMetaInfo>();
        
        foreach (var prop in props)
        {
            if(prop.GetCustomAttribute<DynamicQLExcludeAttribute>() != null)
                continue;
            
            var fieldInfo = propBaseInfoType
                .Clone(this)
                .ImportAttributeInfos(this, prop);
            
            propList.Add(fieldInfo.GraphName, fieldInfo);
            
            if (StaticData.TypeToGraphQLTypeMetaInfoMap
                .TryGetValue(fieldInfo.BaseType, 
                    out var metaInfo))
            {
                fieldInfo.TypeMetaInfo = metaInfo;
            }
            else
            {
                if (!StaticData.LeftOverTypeToGraphQLFieldMetaInfoMap.TryGetValue(fieldInfo.BaseType!, out var list))
                {
                    list = new List<PropertyMetaInfo>();
                    StaticData.LeftOverTypeToGraphQLFieldMetaInfoMap.Add(fieldInfo.BaseType, list);
                }
                list.Add(fieldInfo);
            }
        }

        // Resolve LeftOverTypeToGraphQLFieldMetaInfoMap

        if (StaticData.LeftOverTypeToGraphQLFieldMetaInfoMap.TryGetValue(BaseType, out var resolverList))
        {
            foreach (var fieldInfo in resolverList)
            {
                fieldInfo.TypeMetaInfo = this;
            }
        }

        return propList;
    }
    private Dictionary<string, PropertyMetaInfo> GetSubQueries()
    {
        var subQueries = new Dictionary<string, PropertyMetaInfo>();
        
        foreach (var (name, info) in Properties)
        {
            if (info.PropertyInfo.PropertyType.Namespace == "System") continue;
            
            subQueries.Add(info.GraphName, info);
        }

        return subQueries;
    }

    private Dictionary<string, QueryField> GetQueryFields()
    {
        var queryFields = new Dictionary<string, QueryField>();
        
        foreach (var (name, fieldMetaInfo) in Properties)
        {
            var propertyInfo = fieldMetaInfo.PropertyInfo;
            
            var queryFieldAttributes = propertyInfo.GetCustomAttributes<DynamicQLQueryFieldAttribute>();
            if (!queryFieldAttributes.Any() || propertyInfo.PropertyType.Namespace != "System") continue;
            
            var queryFieldsTemp = QueryField.Parse(fieldMetaInfo);
            foreach (var queryField in queryFieldsTemp)
            {
                queryFields.Add(queryField.Name, queryField);
            }
        }

        return queryFields;
    }

    private static readonly MethodInfo ParseOrderByFieldMethodInfo = typeof(Common)
        .GetMethod(nameof(Common.ParseOrderByField), BindingFlags.Static | BindingFlags.Public)!
        .GetGenericMethodDefinition();
    private List<object> GetOrderByFields()
    {
        var orderByFields = new List<object>();
        foreach (var (name, fieldMetaInfo) in Properties)
        {
            var prop = fieldMetaInfo.PropertyInfo;
            
            var methodInfo = ParseOrderByFieldMethodInfo.MakeGenericMethod(BaseType, prop.PropertyType);
                
            var orderByField = methodInfo.Invoke(null, new object[] { prop });
            if(orderByField != null)
                orderByFields.Add(orderByField);
        }

        return orderByFields;
    }
    
    public static TypeMetaInfo ImportAttributeInfos(Type type, DynamicQLOptions options)
    {
        var typeInfo = new TypeMetaInfo
        {
            Options = options,
            BaseType = GetBaseType(type)
        };

        typeInfo.QueryOptions = typeInfo.GetDynamicQLAttribute();
        typeInfo.PageSize = typeInfo.GetPageSize();

        typeInfo.MinimumExecutionTime = type.GetCustomAttribute<DynamicQLAttribute>()?.MinimumExecutionTime ?? 0;
        
        typeInfo.SingleQueryGraphType = typeInfo.GetSingleGraphType();
        typeInfo.MultiQueryGraphType = typeInfo.GetMultiGraphType();
        
        typeInfo.SingleInputGraphType = typeInfo.GetSingleInputGraphType();
        typeInfo.MultiInputGraphType = typeInfo.GetMultiInputGraphType();
        
        StaticData.BaseTypeToQueryTypeMap.Add(typeInfo.BaseType, typeInfo.SingleQueryGraphType);
        StaticData.BaseTypeToQueryTypeMap.Add(MakeGenericType(typeof(List<>), typeInfo.BaseType), typeInfo.MultiQueryGraphType);

        StaticData.BaseTypeToInputTypeMap.Add(typeInfo.BaseType, typeInfo.SingleInputGraphType);
        StaticData.BaseTypeToInputTypeMap.Add(MakeGenericType(typeof(List<>), typeInfo.BaseType), typeInfo.MultiInputGraphType);

        typeInfo.Properties = typeInfo.GetProperties();
        typeInfo.SubQueries = typeInfo.GetSubQueries();
        typeInfo.QueryFields = typeInfo.GetQueryFields();
        typeInfo.OrderByFields = typeInfo.GetOrderByFields().OrderBy(x => ((dynamic)x).ExecutionOrder).ToList();
        typeInfo.QueryResolvers = typeInfo.GetQueryResolvers(options);
        typeInfo.MutationResolvers = typeInfo.GetMutationResolvers(options);

        typeInfo.ExcludedValidationMiddlewares = type.GetCustomAttributes<DynamicQLExcludeFromValidationMiddleware>()
            .Select(x => x.ExcludedValidationMiddlewareType)
            .ToList();

        typeInfo.ValidationMiddlewares = type.GetCustomAttributes<DynamicQLValidationMiddleware>()
            .Select(x => x.ValidationMiddlewareType)
            .Except(options.DefaultValidationMiddlewareTypes)
            .ToList();
        
        return typeInfo;
    }
}