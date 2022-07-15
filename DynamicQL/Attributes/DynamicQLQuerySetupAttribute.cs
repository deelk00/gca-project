using DynamicQL.Attributes.Enums;

namespace DynamicQL.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class DynamicQLQuerySetupAttribute : Attribute
{
    public SetupType SetupType { get; }
    public int ExecutionOrder { get; }
    public string? ResolverName { get; set; }
    
    public DynamicQLQuerySetupAttribute(SetupType setupType = SetupType.Setup, int executionOrder = 0, string? resolverName = null)
    {
        SetupType = setupType;
        ExecutionOrder = executionOrder;
        ResolverName = resolverName;
    }
}