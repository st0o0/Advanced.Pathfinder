using stuff.graph.net;

namespace Advanced.Pathfinder.Core;

public abstract record EdgeCostRequest(int Id, int Value, ChangeMethod ChangeMethod) : EdgeMessageBase(Id);

public record IncreaseEdgeCost(int Id, int Value) : EdgeCostRequest(Id, Value, ChangeMethod.Increase);
public record DecreaseEdgeCost(int Id, int Value) : EdgeCostRequest(Id, Value, ChangeMethod.Decrease);
public record EdgeCostUpdated(int Id, int UpdatedValue);

public record InitializeEdge(int Id, Edge Edge) : EdgeMessageBase(Id);
public record EdgeInitialized(int Id);
public record ReloadEdge(int Id, Edge Edge) : EdgeMessageBase(Id);
public record EdgeReloaded(int Id, string ErrorMessage = "", bool Success = true);

public record BlockEdgeCommand(int Id) : EdgeMessageBase(Id);
public record UnblockEdgeCommand(int Id) : EdgeMessageBase(Id);

public abstract record EdgeMessageBase(int Id) : IEdgeEntity;
public interface IEdgeEntity : IIdentifier<int> { }