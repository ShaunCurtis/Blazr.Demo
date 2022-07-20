/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public record AppAuthFields
{
    public Guid OwnerId { get; init; } = Guid.Empty;
}

