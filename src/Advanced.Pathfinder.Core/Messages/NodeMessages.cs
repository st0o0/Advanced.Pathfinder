using stuff.graph.net;

namespace Advanced.Pathfinder.Core;

public abstract record NodeCostRequest(int Id, int Value, ChangeMethod ChangeMethod) : NodeMessageBase(Id);
public record IncreaseNodeCost(int Id, int Value) : NodeCostRequest(Id, Value, ChangeMethod.Increase);
public record DecreaseNodeCost(int Id, int Value) : NodeCostRequest(Id, Value, ChangeMethod.Decrease);
public record NodeCostUpdated(int Id, int UpdatedValue) : NodeMessageBase(Id);

public record InitializeNode(int Id, Node Node) : NodeMessageBase(Id);
public record NodeInitialized(int Id) : NodeMessageBase(Id);
public record ReloadNode(int Id, Node Node) : NodeMessageBase(Id);
public record NodeReloaded(int Id, string ErrorMessage = "", bool Success = true) : NodeMessageBase(Id);

public record BlockNodeCommand(int Id) : NodeMessageBase(Id);
public record UnblockNodeCommand(int Id) : NodeMessageBase(Id);

public abstract record NodeMessageBase(int Id) : INodeEntity;
public interface INodeEntity : IIdentifier<int>;
