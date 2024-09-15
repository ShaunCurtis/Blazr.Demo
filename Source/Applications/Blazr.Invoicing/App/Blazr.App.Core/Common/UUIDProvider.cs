using UuidExtensions;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public static class UUIDProvider
{
    public static Guid GetGuid()
    {
        return Uuid7.Guid();
    }
}
