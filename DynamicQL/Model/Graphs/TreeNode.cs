using DynamicQL.Model.Types;

namespace DynamicQL.Model.Graphs;

public class TreeNode
{
    public TypeMetaInfo TypeMetaInfo { get; set; }
    public List<TreeNode> SubNodes { get; set; }
}