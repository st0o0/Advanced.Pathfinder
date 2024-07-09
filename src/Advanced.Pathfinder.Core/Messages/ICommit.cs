namespace Advanced.Pathfinder.Core;

public interface ICommit
{
    internal ChangeMethod ChangeMethod { get; }
    uint AdditionalCost { get; }
}

public class EdgeCommit(uint additionalCost, ChangeMethod change) : ICommit
{
    private readonly ChangeMethod _changeMethod = change;

    public uint AdditionalCost { get; init; } = additionalCost;

    ChangeMethod ICommit.ChangeMethod => _changeMethod;
}

public class NodeCommit(uint additionalCost, ChangeMethod change) : ICommit
{
    private readonly ChangeMethod _changeMethod = change;

    public uint AdditionalCost { get; init; } = additionalCost;

    ChangeMethod ICommit.ChangeMethod => _changeMethod;
}

public enum ChangeMethod
{
    Invalid,
    Increase,
    Decrease,
}