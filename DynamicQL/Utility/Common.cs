using System.Reflection;
using DynamicQL.Attributes;
using DynamicQL.Model.Types;

namespace DynamicQL.Utility;

public class Common
{
    public static OrderByField<TE, TProp>? ParseOrderByField<TE, TProp>(PropertyInfo propertyInfo)
        where TE : class
    {
        var orderByAttribute = propertyInfo.GetCustomAttribute<DynamicQLOrderByAttribute>();
        if (orderByAttribute == null) return null;
        
        var orderByField = new OrderByField<TE, TProp>();
        
        orderByField.PropertyInfo = propertyInfo;
        orderByField.OrderByOptions = orderByAttribute.Options;
        orderByField.ExecutionOrder = orderByAttribute.ExecutionOrder;
        
        return orderByField;
    }
}