﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class DmoInvoiceItemEditContext
{
    private DmoInvoiceItem _baseRecord;
    public InvoiceItemId Id => _baseRecord.InvoiceItemId;

    [TrackState] public string Description { get; set; }
    [TrackState] public decimal Amount { get; set; }

    public DmoInvoiceItem AsRecord =>
        this.GetMutation(_baseRecord);

    public DmoInvoiceItemEditContext(DmoInvoiceItem record)
    {
        _baseRecord = record;
        this.Description = record.Description;
        this.Amount = record.Amount;
    }
    public bool IsDirty => _baseRecord != this.AsRecord;

    public FluxMutationResult<DmoInvoiceItem> Mutate(FluxContext<InvoiceItemId, DmoInvoiceItem> item)
    {
        return FluxMutationResult<DmoInvoiceItem>.Success(GetMutation(item.Item));
    }

    private DmoInvoiceItem GetMutation(DmoInvoiceItem item)
    {
        return item with
        {
            Description = this.Description,
            Amount = this.Amount,
        };
    }
}