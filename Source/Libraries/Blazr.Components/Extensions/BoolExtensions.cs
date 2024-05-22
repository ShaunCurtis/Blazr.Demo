/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Components;

public static class BoolExtensions
{
    public static bool SetTrue(this ref bool value)
    {
        if (value)
        {
            value = true;
            return true;
        }
        return false;
    }
}
