using Akka.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Advanced.Pathfinder.Node;

public class NodeWorker : ReceivePersistentActor
{
    private readonly string _entityId;
    private readonly IServiceScopeFactory _scopeFactory;

    public NodeWorker(string entityId, IServiceScopeFactory serviceScopeFactory)
    {
        _entityId = entityId;
        _scopeFactory = serviceScopeFactory;
    }

    public override string PersistenceId => $"{GetType().Name}_{_entityId}";
}
