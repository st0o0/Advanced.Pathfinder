using System.Text.Json.Serialization;

namespace Advanced.Pathfinder.Core;

public record SerializableGraph(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("edges")] SerializableEdge[] Edges,
    [property: JsonPropertyName("nodes")] SerializableNode[] Nodes    
);
