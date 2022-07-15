using System.Reflection;

namespace DynamicQL.Extensions;

public static class MemberInfoExtension
{
    public static TAttribute? GetAttribute<TAttribute>(this MemberInfo t)
        where TAttribute : Attribute
    {
        return (TAttribute?)Attribute.GetCustomAttribute(t, typeof(TAttribute));
    }
    
    public static List<TAttribute> GetAttributes<TAttribute>(this MemberInfo t)
        where TAttribute : Attribute
    {
        return Attribute.GetCustomAttributes(t)
            .Where(x => x.GetType() == typeof(TAttribute))
            .Cast<TAttribute>()
            .ToList();
    }
}