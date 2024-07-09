using System.Numerics;
using stuff.graph.net;

namespace Advanced.Pathfinder.Core;

public static class Extensions
{
    public static SerializableGraph ToSerializable(this IGraph graph)
        => new(graph.Id, graph.Edges.Select(item => item.ToSerializable()).ToArray(), graph.Nodes.Select(item => item.ToSerializable()).ToArray());
    public static SerializableEdge ToSerializable(this IEdge value)
        => value switch
        {
            DirectedEdge directedEdge => directedEdge.ToSerializable(),
            Edge edge => edge.ToSerializable(),
            _ => throw new NotImplementedException(),
        };
    public static SerializableEdge ToSerializable(this Edge edge)
        => new(edge.Id, edge.StartNode.Id, edge.EndNode.Id, edge.AdditionalRoutingCost);
    public static SerializableEdge ToSerializable(this DirectedEdge edge)
        => new(edge.Id, edge.StartNode.Id, edge.EndNode.Id, edge.AdditionalRoutingCost, edge.Direction);
    public static SerializableNode ToSerializable(this INode node)
        => new(node.Id, node.Location.ToSerializable());
    public static SerializableLocation ToSerializable(this Vector3 vector)
        => new(vector.X, vector.Y, vector.Z);

    public static IGraph To(this SerializableGraph graph)
    {
        var nodes = graph.Nodes.Select(item => item.To()).ToArray();
        var edges = graph.Edges.Select(edge => edge.To(nodes)).ToArray();
        return new Graph() { Id = graph.Id, Edges = edges, Nodes = nodes };
    }

    public static IEdge To(this SerializableEdge edge, IEnumerable<INode> nodes)
    {
        return edge switch
        {
            { Direction: EdgeDirection.OneWay } => CreateDirectedEdge(edge, nodes),
            _ => CreateEdge(edge, nodes),
        };
    }

    private static Edge CreateEdge(SerializableEdge edge, IEnumerable<INode> nodes)
    {
        return new Edge()
        {
            Id = edge.Id,
            StartNode = nodes.First(item => item.Id == edge.StartNodeId),
            EndNode = nodes.First(item => item.Id == edge.EndNodeId),
            AdditionalRoutingCost = edge.AdditionalRoutingCost,
        };
    }
    private static DirectedEdge CreateDirectedEdge(SerializableEdge edge, IEnumerable<INode> nodes)
    {
        return new DirectedEdge()
        {
            Id = edge.Id,
            StartNode = nodes.First(item => item.Id == edge.StartNodeId),
            EndNode = nodes.First(item => item.Id == edge.EndNodeId),
            AdditionalRoutingCost = edge.AdditionalRoutingCost,
            Direction = edge.Direction,
        };
    }

    public static Node To(this SerializableNode node)
        => new() { Id = node.Id, Location = node.Location.To() };
    public static Vector3 To(this SerializableLocation location)
        => new(location.X, location.Y, location.Z);
}