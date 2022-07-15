using DynamicQL.Attributes.Enums;

namespace DynamicQL.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DynamicQLOrderByAttribute : Attribute
{
    public OrderByOptions Options { get; set; }
    public int ExecutionOrder { get; set; }

    public DynamicQLOrderByAttribute(int executionOrder = 0, OrderByOptions options = OrderByOptions.Asc)
    {
        Options = options;
        ExecutionOrder = executionOrder;
    }
}