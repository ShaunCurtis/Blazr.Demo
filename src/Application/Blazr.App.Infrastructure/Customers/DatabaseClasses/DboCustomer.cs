/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.App.Infrastructure;

public sealed record DboCustomer : IStateEntity, ICommandEntity
{
    [Key] public Guid Uid { get; init; } = Guid.Empty;

    [NotMapped] public Blazr.Core.EntityState EntityState { get; init; }

    public string CustomerName { get; init; } = "Not Set";
}
