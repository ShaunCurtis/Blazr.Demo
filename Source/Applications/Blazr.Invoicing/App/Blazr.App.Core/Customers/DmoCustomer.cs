/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public readonly record struct CustomerId : IRecordId
{
    public Guid Value { get; init; }

    public object GetValueObject() => this.Value;

    //public CustomerId()
    //    => this.Value = Guid.Empty;

    public CustomerId(Guid value)
        => this.Value = value;

    public static CustomerId NewEntity => new(Guid.Empty);
}

public record DmoCustomer : ICommandEntity
{
    public CustomerId CustomerId { get; init; } = CustomerId.NewEntity;
    public string CustomerName { get; init; } = string.Empty;
}
