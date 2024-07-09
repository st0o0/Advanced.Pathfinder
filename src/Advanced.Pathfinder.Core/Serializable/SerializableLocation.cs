using System.Text.Json.Serialization;

namespace Advanced.Pathfinder.Core;

public record SerializableLocation(
    [property: JsonPropertyName("x")] float X,
    [property: JsonPropertyName("y")] float Y,
    [property: JsonPropertyName("z")] float Z
    );
