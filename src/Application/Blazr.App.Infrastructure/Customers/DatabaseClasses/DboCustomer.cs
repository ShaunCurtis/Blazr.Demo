/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.App.Core;

public sealed record DboCustomer 
{
    [Key] public Guid Uid { get; init; } = Guid.Empty;

    public string CustomerName { get; init; } = "Not Set";
}
