using Akka.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Advanced.Pathfinder.CostManagement;

public class CostManager : ReceivePersistentActor
{
    private readonly IServiceScopeFactory _scopeFactory;
    public CostManager(IServiceScopeFactory serviceScopeFactory)
    {
        _scopeFactory = serviceScopeFactory;
    }

    public override string PersistenceId => GetType().Name;
}
