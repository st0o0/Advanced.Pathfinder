namespace Advanced.Pathfinder.Node;

internal class NodeState
{
    public static NodeState FromSnapshot(object value) => new();
    public static NodeState FromConfig(object value) => new();
}