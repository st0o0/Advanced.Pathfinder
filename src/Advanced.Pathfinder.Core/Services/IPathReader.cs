using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Path = Advanced.Pathfinder.Core.Persistence.Data.Path;

namespace Advanced.Pathfinder.Core.Services;

public interface IPathReader
{
    IMongoQueryable<Path> Get();
    IMongoQueryable<Path> Get(Guid id);
    Task<long> GetPathCostAsync(Guid id, CancellationToken cancellationToken = default);
    IEnumerable<Path> GetByPathfinderId(Guid pathfinderId, CancellationToken cancellationToken = default);
}

public class PathReader : IPathReader
{
    public PathReader(IMongoCollection<Path> collection)
        => Collection = collection;

    protected IMongoCollection<Path> Collection { get; }

    public IMongoQueryable<Path> Get()
        => Collection.AsQueryable();
    public IMongoQueryable<Path> Get(Guid id)
        => Get().Where(x => x.Id == id);
    public async Task<long> GetPathCostAsync(Guid id, CancellationToken cancellationToken = default)
        => await Get(id).SelectMany(x => x.Directions).SumAsync(x => x.Cost, cancellationToken);
    public IEnumerable<Path> GetByPathfinderId(Guid pathfinderId, CancellationToken cancellationToken = default)
        => Get().Where(x => x.PathfinderId == pathfinderId).ToList(cancellationToken);
}
