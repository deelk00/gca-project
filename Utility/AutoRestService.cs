using Microsoft.AspNetCore.Builder;
using Utility.Graph;

namespace Utility;

public class AutoRestService
{
    private readonly Dictionary<string, GraphType> _graphTypes = new ();
    public IReadOnlyDictionary<string, GraphType> GraphTypes => _graphTypes;
    
    public AutoRestService(params Type[] types)
    {
        foreach (var type in types)
        {
            var g = new GraphType(type);
            _graphTypes.Add(g.Name, g);
        }
    }
}