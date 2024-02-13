namespace SquidEyes.FxData.Models;

public static class MiscExtenders
{
    public static bool IsUnits(this int value) =>
        Units.IsValue(value);

    public static bool IsFolderName(
        this string value, bool mustBeRooted = true)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        try
        {
            _ = new DirectoryInfo(value);

            if (!mustBeRooted)
                return true;
            else
                return Path.IsPathRooted(value);
        }
        catch (ArgumentException)
        {
            return false;
        }
        catch (PathTooLongException)
        {
            return false;
        }
        catch (NotSupportedException)
        {
            return false;
        }
    }
}