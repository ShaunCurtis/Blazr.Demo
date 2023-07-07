/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed class CustomerEditContext : BlazrEditContext<Customer>
{
    [TrackState] public string CustomerName { get; set; } = string.Empty;

    public CustomerEditContext() : base() { }

    protected override void MapToContext(Customer record)
    {
        this.Uid = record.Uid;
        internalStateCode = record.StateCode;
        this.CustomerName = record.CustomerName;
    }

    protected override Customer MapToRecord()
        => new()
        {
            Uid = this.Uid,
            StateCode = this.StateCode,
            CustomerName = this.CustomerName,
        };
}
