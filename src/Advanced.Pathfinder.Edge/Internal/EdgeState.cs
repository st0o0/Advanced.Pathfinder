namespace Advanced.Pathfinder.Edge;

internal class EdgeState
{
    public static EdgeState FromSnapshot(object value) => new();
    public static EdgeState FromConfig(object value) => new();
}
