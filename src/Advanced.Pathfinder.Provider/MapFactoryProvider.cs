namespace Advanced.Pathfinder.Provider;

public class MapFactoryProvider : IMapFactoryProvider
{
    public static MapFactoryProvider Instance { get; } = new();
    public IMapFactory CreateFactory() => new MapFactory();
}
