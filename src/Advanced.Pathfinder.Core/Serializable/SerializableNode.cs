using System.Text.Json.Serialization;

namespace Advanced.Pathfinder.Core;

public record SerializableNode(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("location")] SerializableLocation Location);
