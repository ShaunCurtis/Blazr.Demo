/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.App.Infrastructure;

public sealed record DboProduct : IStateEntity, ICommandEntity
{
    [Key] public Guid Uid { get; init; } = Guid.Empty;

    [NotMapped] public Blazr.Core.EntityState EntityState { get; init; }

    public string ProductCode { get; init; } = "Not Set";

    public string ProductName { get; init; } = "Not Set";

    public decimal ProductUnitPrice { get; init; } = 0;
}
