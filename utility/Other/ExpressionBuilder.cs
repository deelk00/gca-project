using System.Linq.Expressions;

namespace Utility.Other;

public static class ExpressionBuilder
{
    public static Expression<Func<T, bool>> BuildIdExpression<T, TId>(TId id, string idPropName = "id")
    {
        var param = Expression.Parameter(typeof(T), "x");
        var propAccess = Expression.PropertyOrField(param, idPropName);
        var expConstant = Expression.Constant(id);
        var equalExpression = Expression.Equal(propAccess, expConstant);

        var exp = Expression.Lambda<Func<T, bool>>(equalExpression, param);

        return exp;
    }

    public static Expression<Func<T, TSecond>> BuildPropertyAccessExpression<T, TSecond>(string propName)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var propAccess = Expression.PropertyOrField(param, propName);
        return Expression.Lambda<Func<T, TSecond>>(propAccess, param);
    }

    public static Expression<Func<TType, TType>>BuildPropertySetExpression<TType, TProp>(string propName, TProp value)
    {
        var param = Expression.Parameter(typeof(TType), "x");
        var propAccess = Expression.PropertyOrField(param, propName);
        var constant = Expression.Constant(value);
        var propAlloc = Expression.Assign(propAccess, constant);

        return Expression.Lambda<Func<TType, TType>>(propAlloc, param);
    }
}