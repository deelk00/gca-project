using System.Collections.ObjectModel;
using System.Reflection;

namespace DynamicQL.Extensions;

public static class MemberTypeExtension
{
    public static bool IsNullable(this PropertyInfo property) =>
        IsNullable(property.PropertyType, property.DeclaringType, property.CustomAttributes);

    public static bool IsNullable(this FieldInfo field) =>
        IsNullable(field.FieldType, field.DeclaringType, field.CustomAttributes);

    public static bool IsNullable(this ParameterInfo parameter) =>
        IsNullable(parameter.ParameterType, parameter.Member, parameter.CustomAttributes);

    public static bool IsNullable(Type memberType, MemberInfo? declaringType = null, IEnumerable<CustomAttributeData>? customAttributes = null)
    {
        customAttributes ??= memberType.CustomAttributes;
        
        if (memberType.IsValueType)
            return Nullable.GetUnderlyingType(memberType) != null;

        var nullable = customAttributes
            .FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");

        if (nullable != null) return true;

        for (var type = declaringType; type != null; type = type.DeclaringType)
        {
            var context = type.CustomAttributes
                .FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");
            if (context != null &&
                context.ConstructorArguments.Count == 1 &&
                context.ConstructorArguments[0].ArgumentType == typeof(byte))
            {
                return (byte)context.ConstructorArguments[0].Value! == 2;
            }
        }

        // Couldn't find a suitable attribute
        return false;
    }
}