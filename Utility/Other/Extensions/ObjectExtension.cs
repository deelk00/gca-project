using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;
using Utility.Other.Enums;

namespace Utility.Other.Extensions;

public static class ObjectExtension
{
    public static TProp AccessProperty<T, TProp>(this T first, string propName)
    {
        var exp = ExpressionBuilder.BuildPropertyAccessExpression<T, TProp>(propName);
        return exp.Compile().Invoke(first);
    }

    public static TEntity SetProperty<TEntity, T>(this TEntity obj, string propName, T value)
    {
        var exp = ExpressionBuilder.BuildPropertySetExpression<TEntity, T>(propName, value).Compile();
        return exp.Invoke(obj);
    }

    public static List<PropertyInfo> GetAdjacentEntityTables(this object entity)
    {
        var entityType = entity.GetType();
        return entityType
            .GetProperties()
            // filter out unwanted properties
            .Where(p => 
                p.GetCustomAttribute<NotMappedAttribute>() == null
                && p.GetCustomAttribute<JsonIgnoreAttribute>() == null
            )
            .Where(p => !(!p.PropertyType.IsAssignableTo(typeof(IEnumerable))
                          || !p.PropertyType.IsGenericType
                          || p.PropertyType.GetGenericArguments()[0].Namespace?.StartsWith("System") == true))
            .ToList();
    }

    public static object ToResponseDict(this object entity)
    {
        var entityType = entity.GetType();
        var properties = entityType
            .GetProperties()
            // filter out unwanted properties
            .Where(p => 
                p.GetCustomAttribute<NotMappedAttribute>() == null
                && p.GetCustomAttribute<JsonIgnoreAttribute>() == null
                )
            // filter out property types, which are not contained in System
            // includes all primitives, string, enumerables and more
            .Where(p => p.PropertyType.Namespace?.StartsWith("System") == true || p.PropertyType == typeof(string))
            .ToList();

        var tables = properties
            .Where(p => !(!p.PropertyType.IsAssignableTo(typeof(IEnumerable))
                         || !p.PropertyType.IsGenericType
                         || p.PropertyType.GetGenericArguments()[0].Namespace?.StartsWith("System") == true))
            .ToList();

        properties = properties
            .Where(p => !p.PropertyType.IsAssignableTo(typeof(IEnumerable))
                       || !p.PropertyType.IsGenericType
                       || p.PropertyType.GetGenericArguments()[0].Namespace?.StartsWith("System") ==
                       true)
            .ToList();
        
        var resultDict = properties.ToDictionary(
            p => p.Name.ToCase(NamingConvention.LowerCaseCamelCase),
            p => p.GetValue(entity)
            );

        foreach (var table in tables)
        {
            if(table.GetValue(entity) is not IEnumerable list) continue;
            var ids = (from dynamic l in list select l.Id).Cast<Guid>().ToList();
            resultDict.Add(table.Name, ids);
        }
        
        return resultDict;
    }
}