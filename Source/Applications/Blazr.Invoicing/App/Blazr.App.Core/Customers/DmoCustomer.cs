/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed record CustomerId : IEntityKey
{
    public Guid Value { get; init; }

    public object KeyValue => this.Value;

    public CustomerId(Guid value)
        => this.Value = value;

    public static CustomerId NewEntity
        => new(Guid.Empty);
}

public record DmoCustomer : ICommandEntity, IFluxRecord<CustomerId>
{
    public CustomerId CustomerId { get; init; } = new(Guid.Empty);
    public string CustomerName { get; init; } = string.Empty;

    public CustomerId Id =>this.CustomerId;
}
