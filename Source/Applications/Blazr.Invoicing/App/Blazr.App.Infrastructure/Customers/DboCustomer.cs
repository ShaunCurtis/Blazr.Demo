/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public record DboCustomer : ICommandEntity, IKeyedEntity
{
    [Key] public Guid CustomerID { get; init; }
    public string CustomerName { get; init; } = string.Empty;

    public object KeyValue => CustomerID;
}
