/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public static class RecordUtilities
{
    public static object GetIdentity(object value)
    {
        if (value is IEntity guidIdentity)
            return guidIdentity.Uid;

        return new();
    }
}
