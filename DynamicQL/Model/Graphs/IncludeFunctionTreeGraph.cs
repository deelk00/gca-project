using DynamicQL.Model.Types;
using DynamicQL.Utility;

namespace DynamicQL.Model.Graphs;

public class IncludeFunctionTreeGraph
{
    private readonly Dictionary<Type, IncludeFunctionTreeNode> _includeFunctionTreeNodes = new ();

    public IncludeFunctionTreeGraph()
    {
        var resolveTypes = new Dictionary<Type, List<IncludeFunctionTreeNode>>();
        
        foreach (var metaInfo in StaticData.TypeToGraphQLTypeMetaInfoMap.Values)
        {
            var treeNode = new IncludeFunctionTreeNode();

            foreach (var (_ , subQuery) in metaInfo.SubQueries)
            {
                if (_includeFunctionTreeNodes.TryGetValue(subQuery.BaseType, out var subNode))
                {
                    treeNode.SubQueries.Add(subQuery.BaseType, subNode);
                }
                else
                {
                    if (!resolveTypes.TryGetValue(subQuery.BaseType, out var resolveList))
                    {
                        resolveList = new List<IncludeFunctionTreeNode>();
                        resolveTypes.Add(subQuery.BaseType, resolveList);
                    }
                    resolveList.Add(treeNode);
                }
            }
            
            _includeFunctionTreeNodes.Add(metaInfo.BaseType, treeNode);
        }

        foreach (var (typeToResolve, listToResolve) in resolveTypes)
        {
            if (!_includeFunctionTreeNodes.TryGetValue(typeToResolve, out var treeNodeToAdd)) continue;
            
            foreach (var toResolve in listToResolve)
            {
                if(!toResolve.SubQueries.ContainsKey(typeToResolve))
                    toResolve.SubQueries.Add(typeToResolve, treeNodeToAdd);
            }
        }
    }
    
    
}