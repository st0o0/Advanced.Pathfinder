﻿using Advanced.Pathfinder.Core.Configs;

namespace Advanced.Pathfinder.Core.Persistence.Data;

public record PathPoint(int PointId, uint Cost, Direction Direction);
public record Path(Guid Id, Guid PathfinderId, Guid RequestId, IReadOnlyList<PathPoint> Directions);
