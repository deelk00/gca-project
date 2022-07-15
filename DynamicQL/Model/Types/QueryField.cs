using System.Linq.Expressions;
using System.Reflection;
using DynamicQL.Attributes;
using DynamicQL.Attributes.Enums;
using DynamicQL.Extensions;

namespace DynamicQL.Model.Types;

public enum QueryFieldType
{
    String,
    Number,
    Bool
}

public class QueryField
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? DeprecatedReason { get; set; }
    public object? DefaultValue { get; set; }
    private QueryFieldType underlyingType;
    public bool IsNullable { get; set; }
    public QueryFieldOptions Options { get; set; } = QueryFieldOptions.SearchIgnoreCase;
    public PropertyMetaInfo PropertyMetaInfo { get; private set; }

    public QueryField Clone(TypeMetaInfo newTypeInfo, PropertyMetaInfo? newFieldMetaInfo = null)
    {
        return new QueryField()
        {
            Name = Name,
            Description = Description,
            DeprecatedReason = DeprecatedReason,
            DefaultValue = DefaultValue,
            underlyingType = underlyingType,
            Options = Options,
            PropertyMetaInfo = newFieldMetaInfo ?? PropertyMetaInfo.Clone(newTypeInfo)
        };
    }
    
    public Expression ExpressionConcatOr(Expression left, Expression right)
    {
        return left == null ? right : Expression.Or(left, right);
    }

    public Expression<Func<TEntity, bool>> BuildExpression<TEntity>(object value)
    {
        var ignoreCase = Options.HasFlag(QueryFieldOptions.IgnoreCase) 
                         && underlyingType == QueryFieldType.String;
        if (ignoreCase) 
            value = (value as string)!.ToLower();
        
        var paramExpression = Expression.Parameter(typeof(TEntity), "x");
        Expression expression = null;
        
        if (Options.HasFlag(QueryFieldOptions.Equals))
        {
            expression = AddEqualsExpression(expression, paramExpression, value, ignoreCase);
        }
        if (Options.HasFlag(QueryFieldOptions.NotEquals))
        {
            expression = AddNotEqualsExpression(expression, paramExpression, value, ignoreCase);
        }
        
        switch (underlyingType)
        {
            case QueryFieldType.Bool:
                expression = Expression.Lambda<Func<TEntity, bool>>(expression, paramExpression);
                break;
            
            case QueryFieldType.String:
                expression = BuildStringExpression<TEntity>((value as string)!, expression, paramExpression, ignoreCase);
                break;
            
            case QueryFieldType.Number:
                expression = BuildNumericExpression<TEntity>(value as int?, expression, paramExpression);
                break;
            
            default:
                break;
        }
        return (expression as Expression<Func<TEntity, bool>>)!;
    }
    
    private Expression BuildStringExpression<TEntity>(string value, Expression expression, Expression paramExpression, bool ignoreCase)
    {
        if (Options.HasFlag(QueryFieldOptions.StartsWith))
        {
            expression = AddStartsWithExpression(expression, paramExpression, value, ignoreCase);
        }
        if (Options.HasFlag(QueryFieldOptions.EndsWith))
        {
            expression = AddEndsWithExpression(expression, paramExpression, value, ignoreCase);
        }
        if (Options.HasFlag(QueryFieldOptions.Contains))
        {
            expression = AddContainsExpression(expression, paramExpression, value, ignoreCase);
        }

        return Expression.Lambda<Func<TEntity, bool>>(expression, (paramExpression as ParameterExpression)!);
    }

    private Expression BuildNumericExpression<TEntity>(int? value, Expression expression, Expression paramExpression)
    {
        Expression accessProperty = Expression.PropertyOrField(paramExpression, PropertyMetaInfo.PropertyInfo.Name);
        var constant = Expression.Constant(value);
        if (Options.HasFlag(QueryFieldOptions.GreaterThan))
        {
            var exp = Expression.GreaterThan(accessProperty, constant);
            expression = ExpressionConcatOr(expression, exp);
        }

        if (Options.HasFlag(QueryFieldOptions.GreaterThanOrEqual))
        {
            var exp = Expression.GreaterThanOrEqual(accessProperty, constant);
            expression = ExpressionConcatOr(expression, exp);
        }
        
        if (Options.HasFlag(QueryFieldOptions.LessThan))
        {
            var exp = Expression.LessThan(accessProperty, constant);
            expression = ExpressionConcatOr(expression, exp);
        }

        if (Options.HasFlag(QueryFieldOptions.LessThanOrEqual))
        {
            var exp = Expression.LessThanOrEqual(accessProperty, constant);
            expression = ExpressionConcatOr(expression, exp);
        }
        
        return Expression.Lambda<Func<TEntity, bool>>(expression, (paramExpression as ParameterExpression)!);
    }
    
    private Expression AddInvokeToLower(Expression exp)
    {
        return Expression.Call(exp, typeof(string).GetMethod("ToLower", new Type[] { })!);
    }
    
    private Expression AddEqualsExpression(Expression exp, Expression paramExpression, object value, bool ignoreCase)
    {
        Expression accessProperty = Expression.PropertyOrField(paramExpression, PropertyMetaInfo.PropertyInfo.Name);
        if (PropertyMetaInfo.PropertyInfo.PropertyType == (typeof(Nullable<>)
                .GetGenericTypeDefinition()
                .MakeGenericType(value.GetType())))
        {
            accessProperty = Expression.PropertyOrField(accessProperty, "Value");
        }
        if (ignoreCase)
        {
            accessProperty = AddInvokeToLower(accessProperty);
        }
        
        var constant = Expression.Constant(value);
        Expression expression = Expression.Equal(accessProperty, constant);
        expression = ExpressionConcatOr(exp, expression);
        
        return expression;
    }
    private Expression AddNotEqualsExpression(Expression exp, Expression paramExpression, object value, bool ignoreCase)
    {
        Expression accessProperty = Expression.PropertyOrField(paramExpression, PropertyMetaInfo.PropertyInfo.Name);
        if (ignoreCase)
        {
            accessProperty = AddInvokeToLower(accessProperty);
        }
        
        var constant = Expression.Constant(value);
        Expression expression = Expression.NotEqual(accessProperty, constant);

        expression = ExpressionConcatOr(exp, expression);
        
        return expression;
    }
    private Expression AddStringMethodCallExpression(Expression exp, Expression paramExpression, string methodName, object value, bool ignoreCase)
    {
        var constant = Expression.Constant(value);
        Expression accessProperty = Expression.PropertyOrField(paramExpression, PropertyMetaInfo.PropertyInfo.Name);
        if (ignoreCase)
        {
            accessProperty = AddInvokeToLower(accessProperty);
        }
        Expression stringCallExp = Expression.Call(accessProperty, typeof(string).GetMethod(methodName, 
            new Type[] { typeof(string) })!, constant);

        stringCallExp = ExpressionConcatOr(exp, stringCallExp);
        
        return stringCallExp;
    }
    private Expression AddStartsWithExpression(Expression expression, Expression paramExpression, object value, bool ignoreCase) =>
        AddStringMethodCallExpression(expression, paramExpression, "StartsWith", value, ignoreCase);
    private Expression AddEndsWithExpression(Expression expression, Expression paramExpression, object value, bool ignoreCase) =>
        AddStringMethodCallExpression(expression, paramExpression, "EndsWith", value, ignoreCase);
    private Expression AddContainsExpression(Expression expression, Expression paramExpression, object value, bool ignoreCase) =>
        AddStringMethodCallExpression(expression, paramExpression,"Contains", value, ignoreCase);
    
    private QueryField() {}

    public static IEnumerable<QueryField> Parse(PropertyMetaInfo info)
    {
        var queryFieldAttributes = info.PropertyInfo.GetCustomAttributes<DynamicQLQueryFieldAttribute>();
        var orderByAttribute = info.PropertyInfo.GetCustomAttribute<DynamicQLOrderByAttribute>();
        
        var queryFields = new List<QueryField>();
        
        foreach (var queryFieldAttribute in queryFieldAttributes)
        {
            var queryField = new QueryField();

            queryField.PropertyMetaInfo = info;
            queryField.Name = queryFieldAttribute?.Name
                              ?? info.GraphName;
            queryField.Description = queryFieldAttribute?.Description;
            queryField.DeprecatedReason = queryFieldAttribute?.DeprecatedReason;
            queryField.DefaultValue = queryFieldAttribute?.DefaultValue;
            queryField.IsNullable = queryFieldAttribute?.IsNullable ?? true;
            
            if (info.PropertyInfo.PropertyType.IsNumeric())
            {
                queryField.underlyingType = QueryFieldType.Number;
            }
            if (info.PropertyInfo.PropertyType == typeof(string))
            {
                queryField.underlyingType = QueryFieldType.String;
            }
            if (info.PropertyInfo.PropertyType == typeof(bool))
            {
                queryField.underlyingType = QueryFieldType.Bool;
            }

            queryField.Options = queryFieldAttribute?.Options ?? queryField.Options;
        
            queryFields.Add(queryField);
        }

        return queryFields;
    }
}