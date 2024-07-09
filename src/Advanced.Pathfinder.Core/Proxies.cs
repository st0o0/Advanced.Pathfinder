namespace Advanced.Pathfinder.Core;


public record EdgeCostManagerProxy;
public record NodeCostManagerProxy;
public record CostManagerProxy;
public record EdgeWorkerProxy;
public record NodeWorkerProxy;
public record RequestWorkerProxy;

public static class AkkaRole{
    public const string Role = "Pathfinder";
}