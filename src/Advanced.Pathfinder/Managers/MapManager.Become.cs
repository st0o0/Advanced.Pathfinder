using Advanced.Pathfinder.Core.Messages;

namespace Advanced.Pathfinder.Managers;

public partial class MapManager
{
    private void Ready()
    {   
        _logger.Information("[MapManager][READY]");
        CommandAsync<LoadMap>(LoadMapHandler);
        CommandAsync<UpdateMap>(UpdateMapHandler);
        Command<FindPathRequest>(FindPathRequestHandler);
        CommandAny(msg => Stash.Stash());
        Stash.UnstashAll();
    }

    private void Waiting() => CommandAny(msg => Stash.Stash());
}
