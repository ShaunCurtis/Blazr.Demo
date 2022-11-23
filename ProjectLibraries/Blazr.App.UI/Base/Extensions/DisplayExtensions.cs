/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.UI;

public static class DisplayExtensions
{
    public static string AsGlobalDate(this DateTime value)
        => value.ToString("dd-MMM-yyyy");

    public static string AsGlobalDate(this DateTimeOffset value)
        => value.ToString("dd-MMM-yyyy");

    public static string AsGlobalDate(this DateOnly value)
        => value.ToString("dd-MMM-yyyy");

    public static string AsShortGuid(this Guid value)
        => value.ToString().Substring(27);

    public static MarkupString AsDegC(this int value)
    => new MarkupString($"{value} &deg;C");
    public static MarkupString AsDegF(this int value)
    => new MarkupString($"{((value * 9) / 5) + 32} &deg;F");

    public static string AsYesNo(this bool value)
        => value ? "Yes" : "No";

    public static string AsOnOff(this bool value)
        => value ? "On" : "Off";

    public static string AsEnabled(this bool value)
    => value ? "Enabled" : "Disabled";
}
