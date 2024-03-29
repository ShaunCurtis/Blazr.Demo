﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Core.Utilities;
namespace Blazr.App.Core;

public sealed class InvoiceItemEditContext : BlazrEditContext<InvoiceItem>
{
    [TrackState] public int ItemQuantity { get; set; }

    [TrackState] public decimal ItemUnitPrice { get; set; }

    [TrackState] public Guid? InvoiceUid { get; set; }

    [TrackState] public string InvoiceNumber { get; set; } = string.Empty;

    [TrackState] public Guid? ProductUid { get; set; }

    [TrackState] public string ProductCode { get; set; } = string.Empty;

    [TrackState] public string ProductName { get; set; } = string.Empty;

    public InvoiceItemEditContext() : base() { }

    protected override void MapToContext(InvoiceItem record)
    {
        this.Uid = record.Uid;
        internalStateCode = record.StateCode;
        this.ProductUid = record.ProductUid.ToNullableGuid();
        this.ProductCode = record.ProductCode;
        this.ProductName = record.ProductName;
        this.InvoiceUid = record.InvoiceUid.ToNullableGuid();
        this.InvoiceNumber = record.InvoiceNumber;
        this.ItemQuantity = record.ItemQuantity;
        this.ItemUnitPrice = record.ItemUnitPrice;
    }

    protected override InvoiceItem MapToRecord()
        => new()
        {
            Uid = this.Uid,
            StateCode = this.StateCode,
            ProductUid = this.ProductUid.FromNullableGuid(),
            ProductName = this.ProductName,
            ProductCode = this.ProductCode,
            InvoiceUid = this.InvoiceUid.FromNullableGuid(),
            InvoiceNumber = this.InvoiceNumber,
            ItemQuantity = this.ItemQuantity,
            ItemUnitPrice = this.ItemUnitPrice,
        };
}
