using DynamicQL.Services;

namespace DynamicQL.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DynamicQLExcludeFromValidationMiddleware : Attribute
{
    public Type? ExcludedValidationMiddlewareType { get; set; }
    
    public DynamicQLExcludeFromValidationMiddleware(Type? validationMiddlewareType = null)
    {
        if (validationMiddlewareType != null 
            && !validationMiddlewareType.IsAssignableTo(typeof(IDynamicQLValidationMiddleware)))
            throw new Exception(
                $"Provided types must implement the {typeof(IDynamicQLValidationMiddleware).FullName} interface"
                );
        ExcludedValidationMiddlewareType = validationMiddlewareType;
    }
}