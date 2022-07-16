namespace SquidEyes.FxData.Models;

public static class MiscExtenders
{
    public static bool IsUnits(this int value) =>
        Units.IsValue(value);
}
