/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.UI.Editor;

public static class EditControlHelper
{
    public static string GetLabel<TValue>(Expression<Func<TValue>>? expression)
    {
        string? label = "Not Set";

        if (expression is not null)
            label = FieldIdentifier.Create(expression).FieldName;

        return label;
    }
}
