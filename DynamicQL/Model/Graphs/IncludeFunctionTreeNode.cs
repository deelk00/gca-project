using DynamicQL.Model.Types;

namespace DynamicQL.Model.Graphs;

public class IncludeFunctionTreeNode
{
    public Dictionary<Type, IncludeFunctionTreeNode> SubQueries { get; set; } = new ();
}