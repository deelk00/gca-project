using DynamicQL.Services;

namespace DynamicQL.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DynamicQLValidationMiddleware : Attribute
{
    public Type ValidationMiddlewareType { get; set; }
    
    public DynamicQLValidationMiddleware(Type validationMiddlewareType)
    {
        if (!validationMiddlewareType.IsAssignableTo(typeof(IDynamicQLValidationMiddleware)))
            throw new Exception(
                $"Provided types must implement the {typeof(IDynamicQLValidationMiddleware).FullName} interface"
            );
        ValidationMiddlewareType = validationMiddlewareType;
    }
}