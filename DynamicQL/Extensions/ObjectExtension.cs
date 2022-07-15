namespace DynamicQL.Extensions;

public static class ObjectExtension
{
    public static object? GetPropertyValueByName(this object obj, string propertyName)
    {
        var propInfo = obj.GetType().GetProperty(propertyName);

        return propInfo != null ? propInfo.GetValue(obj) : null;
    }
    public static void SetPropertyValueByName(this object obj, string propertyName, object? value)
    {
        var propInfo = obj.GetType().GetProperty(propertyName);

        propInfo?.SetValue(obj, value);
    }
}