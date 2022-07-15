using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using DynamicQL.Attributes;
using DynamicQL.Extensions;
using DynamicQL.Utility;
using GraphQL;
using GraphQL.Types;

namespace DynamicQL.Model.Types;

public class PropertyMetaInfo
{
    public string Name { get; set; }
    public string GraphName { get; set; }
    public bool IsNullable { get; set; }
    public string? Description { get; set; }
    public dynamic? Resolver { get; set; }
    public string? DeprecationReason { get; set; }
    public PropertyInfo PropertyInfo { get; set; }
    public bool IsEnumerable { get; set; }
    public bool IsExcluded { get; set; }
    public Type BaseType { get; set; }
    public Type InputGraphType { get; set; }
    public Type OutputGraphType { get; set; }
    public TypeMetaInfo? TypeMetaInfo { get; set; }

    public override string ToString()
    {
        return $"{Name}, {BaseType.Name}, {InputGraphType.Name}";
    }

    public PropertyMetaInfo Clone(TypeMetaInfo newTypeInfo) 
    {
        return new PropertyMetaInfo()
        {
            Name = Name,
            GraphName = GraphName,
            Description = Description,
            Resolver = Resolver,
            DeprecationReason = DeprecationReason,
            IsEnumerable = IsEnumerable,
            BaseType = BaseType,
            InputGraphType = InputGraphType,
            TypeMetaInfo = newTypeInfo,
            IsExcluded = IsExcluded,
            PropertyInfo = PropertyInfo,
            IsNullable = IsNullable
        };
    }
    
    public PropertyMetaInfo ImportAttributeInfos(TypeMetaInfo type, PropertyInfo propertyInfo)
    {
        TypeMetaInfo = type;
        PropertyInfo = propertyInfo;

        IsEnumerable = propertyInfo.PropertyType.IsAssignableTo(typeof(IEnumerable)) 
                       && propertyInfo.PropertyType != typeof(string);
        
        BaseType = !IsEnumerable ? propertyInfo.PropertyType : propertyInfo.PropertyType.GetGenericArguments()[0];
        
        IsNullable = propertyInfo.GetCustomAttribute<DynamicQLPropertyAttribute>()?.NullableOptions.HasFlag(NullableOptions.IsNullable)
                     ?? propertyInfo.IsNullable();

        if (StaticData.BaseTypeToQueryTypeMap.TryGetValue(
                IsEnumerable ? typeof(List<>).GetGenericTypeDefinition().MakeGenericType(BaseType) : BaseType,
                out var qlType
            ))
        {
            if (IsNullable && qlType.Name == typeof(NonNullGraphType<>).Name)
                qlType = qlType.GetGenericArguments()[0];
        }
        
        if (qlType == null || Nullable.GetUnderlyingType(qlType) != null)
        {
            IsNullable = false;
        }

        var graphQLMetaInfoAttribute = propertyInfo.GetCustomAttribute(typeof(GraphQLMetadataAttribute)) as GraphQLMetadataAttribute;
        var dynamicQLAttribute = propertyInfo.GetCustomAttribute<DynamicQLPropertyAttribute>();

        Name = propertyInfo.GetCustomAttribute<DynamicQLPropertyAttribute>()?.Name
               ?? dynamicQLAttribute?.Name
               ?? graphQLMetaInfoAttribute?.Name
               ?? propertyInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName!;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        var isRenamed = Name != null;
        
        Name ??= propertyInfo.Name;

        GraphName = Name.ToLowerCaseCamelCase();
        
        Description = propertyInfo.GetCustomAttribute<DynamicQLPropertyAttribute>()?.Description
                      ?? propertyInfo.GetCustomAttribute<DynamicQLPropertyAttribute>()?.Description
                      ?? propertyInfo.GetCustomAttribute<GraphQLMetadataAttribute>()?.Description
                      ?? propertyInfo.GetCustomAttribute<DescriptionAttribute>()?.Description 
                      ?? propertyInfo.Description();

        DeprecationReason = propertyInfo.GetCustomAttribute<DynamicQLPropertyAttribute>()?.DeprecationReason
                            ?? graphQLMetaInfoAttribute?.DeprecationReason;

        var queryGraphType = qlType;
        if (propertyInfo.PropertyType.IsEnum)
        {
            qlType = typeof(EnumerationGraphType<>)
                .GetGenericTypeDefinition()
                .MakeGenericType(propertyInfo.PropertyType);
            queryGraphType = qlType;

            if (!StaticData.EnumToGraphQLEnumTypeMap.ContainsKey(propertyInfo.PropertyType))
            {
                StaticData.EnumToGraphQLEnumTypeMap.Add(propertyInfo.PropertyType, qlType);
            }
        }
        else if (qlType == null)
        {
            qlType ??= typeof(QueryType<>)
                .GetGenericTypeDefinition()
                .MakeGenericType(BaseType);
            
            if (IsEnumerable)
            {
                var baseIsNullable = MemberTypeExtension.IsNullable(propertyInfo.PropertyType.GetGenericArguments()[0]);
                if (baseIsNullable)
                {
                    queryGraphType = typeof(ListGraphType<>)
                        .GetGenericTypeDefinition()
                        .MakeGenericType(qlType);

                    qlType = typeof(NonNullGraphType<>)
                        .GetGenericTypeDefinition()
                        .MakeGenericType(qlType);
                }
                
                qlType = typeof(ListGraphType<>)
                    .GetGenericTypeDefinition()
                    .MakeGenericType(qlType);

                queryGraphType ??= qlType;
            }
            else
            {
                queryGraphType = qlType;
            }
            
            if(!IsNullable)
                qlType = typeof(NonNullGraphType<>)
                    .GetGenericTypeDefinition()
                    .MakeGenericType(qlType);
        }
        else
        {
            OutputGraphType = qlType;
        }
        
        InputGraphType = qlType;
        OutputGraphType = queryGraphType;
        
        if (isRenamed)
        {
            Resolver = ResolveExpressionBuilderMethodInfo
                .MakeGenericMethod(TypeMetaInfo.BaseType)
                .Invoke(null, new object[] { this });
        }


        IsExcluded = propertyInfo.GetCustomAttribute<DynamicQLExcludeAttribute>() != null;
            
        return this;
    }

    private static readonly MethodInfo ResolveExpressionBuilderMethodInfo = typeof(PropertyMetaInfo)
        .GetMethod(nameof(ResolveExpressionBuilder), BindingFlags.Static | BindingFlags.NonPublic)!
        .GetGenericMethodDefinition();
    
    private static Expression<Func<T, object?>> ResolveExpressionBuilder<T>(PropertyMetaInfo propertyInfo)
    {
        var param = Expression.Parameter(typeof(T), "x");
        Expression accessProperty = Expression.PropertyOrField(param, propertyInfo.PropertyInfo.Name);
        return Expression.Lambda<Func<T, object?>>(accessProperty, param);
    }
}