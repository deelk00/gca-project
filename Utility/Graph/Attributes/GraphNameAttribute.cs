namespace Utility.Graph.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class GraphNameAttribute : Attribute
{
    public string Name { get; set; }
    public GraphNameAttribute(string name)
    {
        Name = name;
    }
}