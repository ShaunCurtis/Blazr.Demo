/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Components.Editor;

public static class ComponentHelper
{
    public static bool TryGetFieldIdentifier(Expression<Func<string>>? expression, [NotNullWhen(true)] out FieldIdentifier? fi)
    {
        fi = null;
        if (expression is null)
            return false;

        fi = FieldIdentifier.Create(expression);
        return fi is not null;
    }
}
