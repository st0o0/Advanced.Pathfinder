namespace Advanced.Pathfinder.CostManagement;

public class CostManagerState
{
    public static EdgeState FromSnapshot(object value) => new();
    public static EdgeState FromConfig(object value) => new();
}
