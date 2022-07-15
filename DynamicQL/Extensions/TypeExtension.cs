using System.Collections;

namespace DynamicQL.Extensions;

public static class TypeExtension
{
    private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
    {
        typeof(int),  typeof(double),  typeof(decimal),
        typeof(long), typeof(short),   typeof(sbyte),
        typeof(byte), typeof(ulong),   typeof(ushort),  
        typeof(uint), typeof(float)
    };
    
    public static Type? GetEnumerableType(this Type t) {
        return !typeof(IEnumerable).IsAssignableFrom(t) ? null : (
            from it in (new[] { t }).Concat(t.GetInterfaces())
            where it.IsGenericType
            where typeof(IEnumerable<>)==it.GetGenericTypeDefinition()
            from x in it.GetGenericArguments() // x represents the unknown
            let b = it.IsConstructedGenericType // b stand for boolean
            select b ? x : x.BaseType).FirstOrDefault()??typeof(object);
    }

    public static bool IsNumeric(this Type myType)
    {
        return NumericTypes.Contains(Nullable.GetUnderlyingType(myType) ?? myType);
    }
}