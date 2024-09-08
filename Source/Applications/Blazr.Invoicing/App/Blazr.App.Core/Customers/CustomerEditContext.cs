/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class CustomerEditContext : IRecordEditContext<DmoCustomer>
{
    public DmoCustomer BaseRecord { get; private set; }
 
    [TrackState] public string CustomerName { get; set; } = string.Empty;

    public DmoCustomer AsRecord => this.BaseRecord with
    {
        CustomerName = this.CustomerName
    };

    public CustomerEditContext()
    {
        this.BaseRecord = new();
        this.Load(this.BaseRecord);
    }

    public CustomerEditContext(DmoCustomer record)
    {
        this.BaseRecord = record;
        this.Load(record);
    }

    public bool IsDirty => this.BaseRecord != this.AsRecord;

    public IDataResult Load(DmoCustomer record)
    {
        var alreadyLoaded = this.BaseRecord.CustomerId != CustomerId.NewEntity;

        if (alreadyLoaded)
            return DataResult.Failure("A record has already been loaded.  You can't overload it.");

        this.BaseRecord = record;
        this.CustomerName = record.CustomerName;
        return DataResult.Success();
    }
}
