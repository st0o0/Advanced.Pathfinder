using Advanced.Pathfinder.Core.Configs;
using MongoDB.Driver;

namespace Advanced.Pathfinder.Core.Services;

public interface IMapConfigWriter : IMapConfigReader
{
    Task<bool> WriteAsync(MapConfig config, CancellationToken cancellationToken = default);
}

public class MapConfigWriter : MapConfigReader, IMapConfigWriter
{
    public MapConfigWriter(IMongoCollection<MapConfig> collection) : base(collection)
    { }

    public async Task<bool> WriteAsync(MapConfig config, CancellationToken cancellationToken = default)
    {
        var update = Builders<MapConfig>.Update
        .Set(x => x.Id, config.Id)
        .Set(x => x.CollectionIds, config.CollectionIds)
        .Set(x => x.Count, config.Count);

        var result = await Collection.UpdateOneAsync(x => x.Id == config.Id, update, new UpdateOptions() { IsUpsert = true }, cancellationToken);
        return result.IsAcknowledged;
    }
}
