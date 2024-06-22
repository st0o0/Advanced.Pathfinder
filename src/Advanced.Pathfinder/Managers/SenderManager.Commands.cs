using Advanced.Pathfinder.Core.Messages;
using Akka.Actor;

namespace Akka.Pathfinder.Managers;

public partial class SenderManager
{
    private void SavePathfinderSenderHandler(SavePathfinderSender msg)
    {
        _logger.Verbose("[SenderManager][{MessageType}] received", msg.GetType().Name);
        _pathfinderSender.Add(msg.PathfinderId, Sender);
    }

    private void ForwardToPathfinderSenderHandler(ForwardToPathfinderSender msg)
    {
        _logger.Verbose("[SenderManager][{MessageType}] received", msg.GetType().Name);
        if (_pathfinderSender.TryGetValue(msg.PathfinderId, out var sender))
        {
            sender.Forward(msg.Message);
            _pathfinderSender.Remove(msg.PathfinderId);
        }
    }
}
