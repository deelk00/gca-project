using System.Collections;
using System.Reflection;
using Utility.Graph.Attributes;

namespace Utility.Graph;

public class GraphType
{
    public string Name { get; set; }
    public Type BaseType { get; set; }
    public Dictionary<string, GraphType> AdjacentTypes { get; set; } = new ();

    public GraphType(Type propType)
    {
        Name = propType.GetCustomAttribute<GraphNameAttribute>()?.Name
            ?? propType.Name;

        var complexProps = propType
            .GetProperties()
            .Where(propInfo => !propInfo.PropertyType.IsPrimitive()
                || (propInfo.PropertyType.IsAssignableTo(typeof(IEnumerable))
                    && !propInfo.PropertyType.IsGenericType
                    && !propInfo.PropertyType.GetGenericArguments()[0].IsPrimitive));

        foreach (var complexProp in complexProps)
        {
            var g = new GraphType(complexProp.PropertyType);
            AdjacentTypes.Add(g.Name, g);
        }
        
        BaseType = propType;
    }
}