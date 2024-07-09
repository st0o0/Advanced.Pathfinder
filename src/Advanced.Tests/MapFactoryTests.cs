using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Advanced.Pathfinder.Core;
using Advanced.Pathfinder.Provider;
using stuff.graph.net;

namespace Advanced.Tests;

public class MapFactoryTests
{
    [Fact]
    public void JsonConvert()
    {
        var value = File.ReadAllText("newmap.json");
        const int expectedNodes = 4845;
        const int expectedEdges = 5593;
        var serializableGraph = JsonSerializer.Deserialize<SerializableGraph>(value);
        Assert.NotNull(serializableGraph);
        Assert.Equal(expectedEdges, serializableGraph.Edges.Length);
        Assert.Equal(expectedNodes, serializableGraph.Nodes.Length);
        var graph = serializableGraph.To();
        Assert.NotNull(graph);
        Assert.Equal(expectedEdges, graph.Edges.Length);
        Assert.Equal(expectedNodes, graph.Nodes.Length);
    }

    [Fact]
    public void Map0Tests()
    {
        var mapConfig = MapProvider.Layouts[0];
        var edges = mapConfig.Edges;
        var nodes = mapConfig.Nodes;
        Assert.NotNull(mapConfig);
        Assert.NotEqual(Guid.Empty, mapConfig.Id);
        Assert.NotNull(mapConfig);
        Assert.NotEmpty(edges);
        Assert.Single(edges);
        Assert.NotEmpty(nodes);
        Assert.Equal(2, nodes.Length);
        Assert.Equal(EdgeDirection.OneWay, edges[0].GetDirection());
    }

    [Fact]
    public void CreateRandomMapConfig()
    {
        int x = 3;
        var mapConfig = MapFactoryProvider.Instance.CreateFactory().Create(new MapSettings(42, 20, new Vector3(x, x, x)), true);

        Assert.NotNull(mapConfig);
        Assert.NotEqual(Guid.Empty, mapConfig.Id);
        Assert.NotNull(mapConfig);
        Assert.NotEmpty(mapConfig.Nodes);
        Assert.NotEmpty(mapConfig.Edges);
        var t = mapConfig.Edges[9];
        Assert.Single(mapConfig.Edges.Where(item => item.StartNode == t.StartNode && item.EndNode == t.EndNode));
        Assert.Equal(27, mapConfig.Nodes.Length);
        Assert.Equal(90, mapConfig.Edges.Length);
    }

    [Fact]
    public void CreateMapConfigFromArray()
    {
        var mapSize = new Vector3(3, 3, 2);
        var mapSettings = new MapSettings(42, 20, mapSize);

        int[,,] kekw = new int[,,]
        {
            {
                { 1, 1, 1 },
                { 1, 0, 1 },
                { 1, 1, 1 }
            },
            {
                { 1, 1, 1 },
                { 1, 1, 1 },
                { 0, 0, 0 }
            }
        };

        var mapConfig = MapFactoryProvider.Instance.CreateFactory().Create(mapSettings, kekw);

        Assert.NotNull(mapConfig);
        Assert.NotEqual(Guid.Empty, mapConfig.Id);
        Assert.NotNull(mapConfig);
        Assert.NotEmpty(mapConfig.Nodes);
        Assert.NotEmpty(mapConfig.Edges);
        Assert.Equal(14, mapConfig.Nodes.Length);
        Assert.Equal(30, mapConfig.Edges.Length);
    }

    [Fact]
    public void CreateLargeMapConfig()
    {
        int x = 100;
        var mapConfig = MapFactoryProvider.Instance.CreateFactory().Create(new MapSettings(42, 20, new Vector3(x, x, x)), true);

        Assert.NotNull(mapConfig);
        Assert.NotEqual(Guid.Empty, mapConfig.Id);
        Assert.NotNull(mapConfig);
        Assert.NotEmpty(mapConfig.Nodes);
        Assert.NotEmpty(mapConfig.Edges);
        Assert.Equal(1000000, mapConfig.Nodes.Length);
        Assert.Equal(5910300, mapConfig.Edges.Length);
    }
}