using stuff.graph.net;

namespace Advanced.Pathfinder.Provider;

public interface IMapFactory
{
    Graph Create(MapSettings mapSettings, bool intergalacticDummyMode = false);
    Graph Create(MapSettings mapSettings, int[,,] map);
}
