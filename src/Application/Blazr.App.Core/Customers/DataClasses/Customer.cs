/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.App.Core;

public sealed record Customer : IGuidIdentity, IStateEntity, ICommandEntity
{
    [Key] public Guid Uid { get; init; } = Guid.Empty;

    [NotMapped] public int StateCode { get; init; } = 1;

    public string CustomerName { get; init; } = "Not Set";
}
