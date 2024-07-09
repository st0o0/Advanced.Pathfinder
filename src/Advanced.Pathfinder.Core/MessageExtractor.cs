using Akka.Cluster.Sharding;

namespace Advanced.Pathfinder.Core;

public class MessageExtractor(int maxShard = 50) : HashCodeMessageExtractor(maxShard)
{
    public override string EntityId(object message)
        => message is IEntityId ntt ? ntt.EntityId : throw new ArgumentException("DUMMKOPF");
}