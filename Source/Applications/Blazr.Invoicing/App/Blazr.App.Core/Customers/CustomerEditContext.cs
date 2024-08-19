/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class CustomerEditContext
{
    private DmoCustomer _baseRecord;
    public CustomerId Id => _baseRecord.CustomerId;

    [TrackState] public string CustomerName { get; set; } = string.Empty;

    public DmoCustomer AsRecord =>
        _baseRecord with
        {
            CustomerName = this.CustomerName
        };

    public CustomerEditContext(DmoCustomer record)
    {
        _baseRecord = record;
        this.Load(record);
    }

    public bool IsDirty => _baseRecord != this.AsRecord;

    protected void Load(DmoCustomer record)
    {
        this.CustomerName = record.CustomerName;
    }

    public DiodeMutationResult<DmoCustomer> Mutate(DiodeContext<CustomerId, DmoCustomer> item)
    {
        var record = item.Item with
        {
            CustomerName = this.CustomerName
        };
        return DiodeMutationResult<DmoCustomer>.Success(record);
    }
}
