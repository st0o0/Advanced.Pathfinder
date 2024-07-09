using Akka.Persistence;

namespace Advanced.Pathfinder.Actors;

public class RequestWorker : ReceivePersistentActor
{
    private readonly string _entityId;

    public RequestWorker(string entityId)
    {
        _entityId = entityId;
    }

    public override string PersistenceId => $"{GetType().Name}_{_entityId}";
}
