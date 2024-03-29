﻿/// ============================================================
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
        internalStateCode = record.StateCode;
        this.CustomerUid = record.CustomerUid;
        this.CustomerName = record.CustomerName;
        this.InvoiceNumber = record.InvoiceNumber;
        this.InvoiceDate = record.InvoiceDate;
        this.InvoicePrice = record.InvoicePrice;
    }

    protected override Invoice MapToRecord()
        => new()
        {
            Uid = this.Uid,
            StateCode = this.StateCode,
            CustomerUid = this.CustomerUid,
            CustomerName = this.CustomerName,
            InvoiceNumber = this.InvoiceNumber,
            InvoiceDate = this.InvoiceDate ?? DateOnly.MinValue,
            InvoicePrice = this.InvoicePrice,
        };
}
