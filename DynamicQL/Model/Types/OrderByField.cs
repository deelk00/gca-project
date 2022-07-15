using System.Linq.Expressions;
using System.Reflection;
using DynamicQL.Attributes;
using DynamicQL.Attributes.Enums;

namespace DynamicQL.Model.Types;

public class OrderByField<TEntity, TProperty>
    where TEntity : class
{
    public PropertyInfo PropertyInfo { get; set; }
    public OrderByOptions OrderByOptions { get; set; }
    public int ExecutionOrder { get; set; }
    public Expression<Func<TEntity, TProperty>>? OrderByExpression { get; set; }

    public OrderByField<TEntity, TProperty> Clone()
    {
        return new OrderByField<TEntity, TProperty>()
        {
            PropertyInfo = PropertyInfo,
            OrderByOptions = OrderByOptions,
            ExecutionOrder = ExecutionOrder,
            OrderByExpression = OrderByExpression
        };
    }

    public IOrderedQueryable<TEntity> RunOrderBy(IQueryable<TEntity> query)
    {
        OrderByExpression ??= BuildOrderByExpression();
        
        return OrderByOptions.Asc == OrderByOptions 
            ? query.OrderBy(OrderByExpression) 
            : query.OrderByDescending(OrderByExpression);
    }

    public IOrderedQueryable<TEntity> RunThenBy(IOrderedQueryable<TEntity> query)
    {
        OrderByExpression ??= BuildOrderByExpression();
        
        return OrderByOptions.Asc == OrderByOptions 
            ? query.ThenBy(OrderByExpression) 
            : query.ThenByDescending(OrderByExpression);
    }

    public Expression<Func<TEntity, TProperty>> BuildOrderByExpression() 
    {
        var paramExp = Expression.Parameter(typeof(TEntity), "x");
        var propertyExpr = Expression.PropertyOrField(paramExp, PropertyInfo.Name);
        return Expression.Lambda<Func<TEntity, TProperty>>(propertyExpr, paramExp);
    }
}