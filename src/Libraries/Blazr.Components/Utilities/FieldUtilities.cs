/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Components;

public static class FieldUtilities
{
    /// <summary>
    /// Method to convert the supplied object into a MarkupString
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static MarkupString GetAsMarkup(object? value)
    {
        switch (value)
        {
            case MarkupString mValue:
                return mValue;

            case string sValue:
                return (MarkupString)(sValue);

            case null:
                return new MarkupString(string.Empty);

            default:
                return new MarkupString(value?.ToString() ?? String.Empty);
        }
    }
}
