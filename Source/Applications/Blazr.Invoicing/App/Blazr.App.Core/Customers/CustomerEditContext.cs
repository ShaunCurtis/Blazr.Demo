/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class CustomerEditContext : IRecordEditContext<DmoCustomer>
{
    private DmoCustomer _baseRecord;
    public CustomerId Id => _baseRecord.CustomerId;

    [TrackState] public string CustomerName { get; set; } = string.Empty;

    public DmoCustomer AsRecord => _baseRecord with
    {
        CustomerName = this.CustomerName
    };

    public CustomerEditContext()
    {
        _baseRecord = new();
        this.Load(_baseRecord);
    }

    public CustomerEditContext(DmoCustomer record)
    {
        _baseRecord = record;
        this.Load(record);
    }

    public bool IsDirty => _baseRecord != this.AsRecord;

    public IDataResult Load(DmoCustomer record)
    {
        var alreadyLoaded = _baseRecord.CustomerId != CustomerId.NewEntity;

        if (alreadyLoaded)
            return DataResult.Failure("A record has already been loaded.  You can overload it.");
 
        _baseRecord = record;
        this.CustomerName = record.CustomerName;
        return DataResult.Success();
    }
}
