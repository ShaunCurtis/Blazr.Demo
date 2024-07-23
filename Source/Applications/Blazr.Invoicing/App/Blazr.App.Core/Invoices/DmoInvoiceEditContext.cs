/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class DmoInvoiceEditContext
{
    private DmoInvoice _baseRecord;
    public InvoiceId Id => _baseRecord.InvoiceId;

    [TrackState] public CustomerLookUpItem? Customer { get; set; }

    // We use a DateTime here as some edit controls only like DateTime
    [TrackState] public DateTime? Date { get; set; } = DateTime.Now;

    public DmoInvoice AsRecord =>
        this.GetMutation(_baseRecord);

    public bool IsCustomerClean => Customer is not null ? Customer.Id.Equals(_baseRecord.CustomerId.Value) : true;

    public DmoInvoiceEditContext(DmoInvoice record)
    {
        _baseRecord = record;
        this.Customer = new() { Id = record.CustomerId.Value, Name = record.CustomerName };
        this.Date = record.Date.ToDateTime(TimeOnly.MinValue);
    }
    public bool IsDirty => _baseRecord != this.AsRecord;

    public FluxMutationResult<DmoInvoice> Mutate(FluxContext<InvoiceId, DmoInvoice> item)
    {
        return FluxMutationResult<DmoInvoice>.Success(GetMutation(item.Item));
    }

    private DmoInvoice GetMutation(DmoInvoice item)
    {
        return item with
        {
            CustomerId = new(this.Customer?.Id ?? Guid.Empty),
            CustomerName = this.Customer?.Name ?? "Not Set",
            Date = DateOnly.FromDateTime(this.Date ?? DateTime.Now ),
        };
    }
}
