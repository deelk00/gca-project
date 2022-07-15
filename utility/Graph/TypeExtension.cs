namespace Utility.Graph;

public static class TypeExtension
{
    public static bool IsPrimitive(this Type type)
    {
        return type.IsPrimitive || type == typeof(string);
    }
}