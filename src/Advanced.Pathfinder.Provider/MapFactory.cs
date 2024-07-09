using System.Numerics;
using stuff.graph.net;

namespace Advanced.Pathfinder.Provider;
public record MapSize(int X, int Y, int Z);

public record MapSettings(int NodeCost, int EdgeCost, Vector3 Size, int Seed = 0);

public class MapFactory : IMapFactory
{
    public Graph Create(MapSettings mapSettings, bool intergalacticDummyMode = false)
    {
        var random = InitializeRandom(mapSettings);
        var map = InitializeMap(mapSettings, random, intergalacticDummyMode);
        var indexBasedMap = ConvertToIndexBasedMap(map);
        return ConvertToMapConfig(indexBasedMap, mapSettings);
    }

    public Graph Create(MapSettings mapSettings, int[,,] map)
    {
        var indexBasedMap = ConvertToIndexBasedMap(map);
        return ConvertToMapConfig(indexBasedMap, mapSettings);
    }

    private static Random InitializeRandom(MapSettings? settings)
    {
        int seedToUse = settings?.Seed ?? 0;

        if (seedToUse == 0)
        {
            seedToUse = DateTime.UtcNow.Microsecond;
        }

        return new Random(seedToUse);
    }

    private static Graph ConvertToMapConfig(int[,,] map, MapSettings settings)
    {
        var listOfNodes = new Dictionary<Vector3, Node>();

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int z = 0; z < map.GetLength(2); z++)
                {
                    if (map[x, y, z] == 0)
                    {
                        continue;
                    }
                    var key = new Vector3(x, y, z);
                    listOfNodes.Add(key, new Node()
                    {
                        Id = map[x, y, z],
                        Location = key
                    });


                }
            }
        }
        
        var listOfEdges = listOfNodes.AsParallel().WithDegreeOfParallelism(8).SelectMany((item, index) => GetEdges(item.Value, index + 6, map, listOfNodes, settings)).ToList();
        return new Graph() { Id = Guid.NewGuid(), Edges = [.. listOfEdges], Nodes = [.. listOfNodes.Values] };
    }

    private static IEnumerable<Edge> GetEdges(Node node, int index, int[,,] map, Dictionary<Vector3, Node> allNodes, MapSettings settings)
    {
        int x = (int)node.Location.X;
        int y = (int)node.Location.Y;
        int z = (int)node.Location.Z;
        var directionsToCheck = new List<Direction>();
        if (x > 0) directionsToCheck.Add(Direction.North);
        if (y > 0) directionsToCheck.Add(Direction.West);
        if (z > 0) directionsToCheck.Add(Direction.Bottom);

        if (x < map.GetLength(0)) directionsToCheck.Add(Direction.South);
        if (y < map.GetLength(1)) directionsToCheck.Add(Direction.East);
        if (z < map.GetLength(2)) directionsToCheck.Add(Direction.Top);
        foreach (Direction direction in directionsToCheck)
        {
            switch (direction)
            {
                case Direction.North:
                    {
                        if (x - 1 <= -1 || y <= -1) break;
                        if (!allNodes.TryGetValue(new Vector3(x, y - 1, z), out var endNode)) break;
                        yield return new Edge()
                        {
                            StartNode = node,
                            EndNode = endNode,
                            AdditionalRoutingCost = settings.EdgeCost,
                            Id = index
                        };
                        index++;
                        break;
                    }
                case Direction.South:
                    {
                        if (x + 1 <= -1 || y <= -1) break;
                        if (!allNodes.TryGetValue(new Vector3(x, y + 1, z), out var endNode)) break;
                        yield return new Edge()
                        {
                            StartNode = node,
                            EndNode = allNodes[new Vector3(x, y + 1, z)],
                            AdditionalRoutingCost = settings.EdgeCost,
                            Id = index
                        };
                        index++;
                        break;
                    }
                case Direction.West:
                    {
                        if (x <= -1 || y - 1 <= -1) break;
                        if (!allNodes.TryGetValue(new Vector3(x, y, z - 1), out var endNode)) break;
                        yield return new Edge()
                        {
                            StartNode = node,
                            EndNode = endNode,
                            AdditionalRoutingCost = settings.EdgeCost,
                            Id = index
                        };
                        index++;
                        break;
                    }
                case Direction.East:
                    {
                        if (x <= -1 || y + 1 <= -1) break;
                        if (!allNodes.TryGetValue(new Vector3(x, y, z + 1), out var endNode)) break;
                        yield return new Edge()
                        {
                            StartNode = node,
                            EndNode = endNode,
                            AdditionalRoutingCost = settings.EdgeCost,
                            Id = index
                        };
                        index++;
                        break;
                    }
                case Direction.Top:
                    {
                        if (x <= -1 || y <= -1) break;
                        if (!allNodes.TryGetValue(new Vector3(x + 1, y, z), out var endNode)) break;
                        yield return new Edge()
                        {
                            StartNode = node,
                            EndNode = endNode,
                            AdditionalRoutingCost = settings.EdgeCost,
                            Id = index
                        };
                        index++;
                        break;
                    }
                case Direction.Bottom:
                    {
                        if (x <= -1 || y <= -1) break;
                        if (!allNodes.TryGetValue(new Vector3(x - 1, y, z), out var endNode)) break;
                        yield return new Edge()
                        {
                            StartNode = node,
                            EndNode = endNode,
                            AdditionalRoutingCost = settings.EdgeCost,
                            Id = index
                        };
                        index++;
                        break;
                    }
            }
        }
    }

    private static int[,,] InitializeMap(MapSettings settings, Random? random = default, bool intergalacticDummyMode = false)
    {
        var result = new int[((int)settings.Size.Y), (int)settings.Size.X, (int)settings.Size.Z];
        for (int x = 0; x < result.GetLength(0); x++)
        {
            for (int y = 0; y < result.GetLength(1); y++)
            {
                for (int z = 0; z < result.GetLength(2); z++)
                {
                    if (intergalacticDummyMode)
                    {
                        result[x, y, z] = 1;
                    }
                    else
                    {
                        result[x, y, z] = random?.Next(0, 2) == 1 ? 1 : 0;
                    }
                }
            }
        }

        return result;
    }

    private static int[,,] ConvertToIndexBasedMap(int[,,] map)
    {
        int index = 0;
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int z = 0; z < map.GetLength(1); z++)
            {
                for (int x = 0; x < map.GetLength(2); x++)
                {
                    index++;
                    map[y, z, x] = map[y, z, x] switch
                    {
                        1 => index,
                        _ => 0,
                    };
                }
            }
        }

        return map;
    }

    public enum Direction : ushort
    {
        North,
        East,
        Top,
        South,
        West,
        Bottom,
        NotSet = 999,
    }
}