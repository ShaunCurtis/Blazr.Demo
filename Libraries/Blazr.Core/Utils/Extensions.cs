/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public static class GuidExtensions
{
    public static readonly Guid Null = new Guid("99999999-9999-9999-9999-999999999999");
 
    public static bool IsNull(this Guid guid)
         => guid == GuidExtensions.Null;

    public static bool IsNotNull(this Guid guid)
        => guid != GuidExtensions.Null;
}

