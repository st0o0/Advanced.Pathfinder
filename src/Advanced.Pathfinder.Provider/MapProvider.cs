using System.Numerics;
using stuff.graph.net;

namespace Advanced.Pathfinder.Provider;

public class MapProvider
{
    private const int BaseCost = 42;

    private static readonly MapFactoryProvider _factoryProvider = MapFactoryProvider.Instance;

    public static Dictionary<int, Graph> Layouts => new()
        {
            { 0, Map0},
            { 1, Map1},
            { 2, _factoryProvider.CreateFactory().Create(new MapSettings(BaseCost, BaseCost*2, new Vector3(3, 3, 3),  15), true)},
            { 3, _factoryProvider.CreateFactory().Create(new MapSettings(BaseCost, BaseCost*2, new Vector3(15, 15, 15), 20), true)},
            { 4, _factoryProvider.CreateFactory().Create(new MapSettings(BaseCost, BaseCost*2, new Vector3(25, 25, 25), 20), true)},
            { 5, _factoryProvider.CreateFactory().Create(new MapSettings(BaseCost, BaseCost*2, new Vector3(35, 40, 40), 20), true)}
        };

    private static Graph Map0
    {
        get
        {
            var nodes = new List<Node>()
            {
                new() { Id = 1, Location = new Vector3(1, 0, 0) },
                new() { Id = 2, Location = new Vector3(0, 1, 0) }
            }.ToArray();
            var edges = new List<Edge>()
            {
                new DirectedEdge(){ Id = 1, Direction = EdgeDirection.OneWay, StartNode = nodes[0], EndNode = nodes[1] }
            }.ToArray();
            return new Graph { Id = Guid.NewGuid(), Edges = edges, Nodes = nodes };
        }
    }

    private static Graph Map1
    {
        get
        {
            // var t = new MapConfigWithPoints(Guid.NewGuid(), new Dictionary<Guid, List<PointConfig>>(){{ Guid.NewGuid(), new List<PointConfig>()
            // {
            //     new(1, BaseCost, new Dictionary<Direction, DirectionConfig>()
            //     {
            //         { Direction.Left, new DirectionConfig(8, BaseCost) },
            //     }),
            //     new(2, BaseCost, new Dictionary<Direction, DirectionConfig>()
            //     {
            //         { Direction.Bottom, new DirectionConfig(1, BaseCost) },
            //         { Direction.Left, new DirectionConfig(9, BaseCost) },
            //     }),
            //     new(3, BaseCost, new Dictionary<Direction, DirectionConfig>()
            //     {
            //         { Direction.Bottom, new DirectionConfig(2, BaseCost) },
            //     }),
            //     new(4, BaseCost, new Dictionary<Direction, DirectionConfig>()
            //     {
            //         { Direction.Right, new DirectionConfig(3, BaseCost) },
            //         { Direction.Left, new DirectionConfig(5, BaseCost) },
            //     }),
            //     new(5, BaseCost, new Dictionary<Direction, DirectionConfig>()
            //     {
            //         { Direction.Bottom, new DirectionConfig(5, BaseCost) },
            //     }),
            //     new(6, BaseCost, new Dictionary<Direction, DirectionConfig>()
            //     {
            //         { Direction.Bottom, new DirectionConfig(7, BaseCost) },
            //         { Direction.Right, new DirectionConfig(9, BaseCost) },
            //     }),
            //     new(7,BaseCost, new Dictionary<Direction, DirectionConfig>()
            //     {
            //         { Direction.Right, new DirectionConfig(8, BaseCost) },
            //     }),
            //     new(8, BaseCost,new Dictionary<Direction, DirectionConfig>()
            //     {
            //         { Direction.Top, new DirectionConfig(9, BaseCost) },
            //     }),
            //     new(9, BaseCost, new Dictionary<Direction, DirectionConfig>()
            //     {
            //         { Direction.Top, new DirectionConfig(4, BaseCost) },
            //     })
            // }}});


            var listOfNodes = Array.Empty<Node>();
            var listOfEdges = Array.Empty<Edge>();
            return new Graph { Id = Guid.NewGuid(), Edges = listOfEdges, Nodes = listOfNodes };
        }
    }
}