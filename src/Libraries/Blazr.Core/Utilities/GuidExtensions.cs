
using System.Runtime.CompilerServices;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core.Utilities
{
    public static class GuidExtensions
    {
        public static Guid? ToNullableGuid(this Guid id) 
            => id == Guid.Empty ? null : id;

        public static Guid FromNullableGuid(this Guid? id)
            => id ?? Guid.Empty;
    }
}
