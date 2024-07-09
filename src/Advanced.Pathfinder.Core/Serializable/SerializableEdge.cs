using System.Text.Json.Serialization;
using stuff.graph.net;

namespace Advanced.Pathfinder.Core;

public record SerializableEdge(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("startNodeId")] int StartNodeId,
    [property: JsonPropertyName("endNodeId")] int EndNodeId,
    [property: JsonPropertyName("additionalRoutingCost")] int AdditionalRoutingCost,
    [property: JsonPropertyName("allowedDirection")] EdgeDirection Direction = EdgeDirection.TwoWay);
