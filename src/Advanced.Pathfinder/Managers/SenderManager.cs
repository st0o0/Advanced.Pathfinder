using Advanced.Pathfinder.Core.Messages;
using Akka.Actor;

namespace Akka.Pathfinder.Managers;

public partial class SenderManager : ReceiveActor
{
    private readonly Serilog.ILogger _logger = Serilog.Log.Logger.ForContext<SenderManager>();
    private readonly Dictionary<Guid, IActorRef> _pathfinderSender = [];
    
    public SenderManager()
    {
        Receive<SavePathfinderSender>(SavePathfinderSenderHandler);
        Receive<ForwardToPathfinderSender>(ForwardToPathfinderSenderHandler);
    }
}
