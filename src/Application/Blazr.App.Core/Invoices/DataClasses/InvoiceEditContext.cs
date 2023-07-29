/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed class InvoiceEditContext : BlazrEditContext<Invoice>
{
    [TrackState] public decimal InvoicePrice { get; set; }

    [TrackState] public string InvoiceNumber { get; set; } = string.Empty;

    [TrackState] public DateOnly? InvoiceDate { get; set; }

    [TrackState] public Guid CustomerUid { get; set; }

    [TrackState] public string CustomerName { get; set; } = string.Empty;
    
    public InvoiceEditContext() : base() { }

    protected override void MapToContext(Invoice record)
    {
        this.Uid = record.Uid;
        this.CustomerUid = record.CustomerUid.Value;
        this.CustomerName = record.CustomerName;
        this.InvoiceNumber = record.InvoiceNumber;
        this.InvoiceDate = record.InvoiceDate;
        this.InvoicePrice = record.InvoicePrice;
    }

    protected override Invoice MapEditFieldsToRecord()
        => this.BaseRecord with 
        {
            CustomerUid = new(this.CustomerUid),
            CustomerName = this.CustomerName,
            InvoiceNumber = this.InvoiceNumber,
            InvoiceDate = this.InvoiceDate ?? DateOnly.MinValue,
            InvoicePrice = this.InvoicePrice,
        };

    protected override Invoice MapEditFieldsAndStateToRecord()
        => MapEditFieldsToRecord() with { EntityState = this.EntityState };
}
