namespace Advanced.Pathfinder.CostManagement;

public class CostManagerState
{
    public static CostManagerState FromSnapshot(object value) => new();
    public static CostManagerState FromConfig(object value) => new();
}
