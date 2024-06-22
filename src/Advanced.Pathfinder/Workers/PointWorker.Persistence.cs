﻿using Path = Advanced.Pathfinder.Core.Persistence.Data.Path;

namespace Advanced.Pathfinder.Workers;

public partial class PointWorker
{
    private void PersistState()
    {
        var persistedWorkerState = _state.GetPersistenceState();
        SaveSnapshot(persistedWorkerState);
    }

    private bool PersistPath(Path path)
        => _pathWriter.Write(path);
}

