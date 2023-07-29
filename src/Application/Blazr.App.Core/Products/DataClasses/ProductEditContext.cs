﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed class ProductEditContext : BlazrEditContext<Product>
{
    [TrackState] public string ProductName { get; set; } = string.Empty;

    [TrackState] public string ProductCode { get; set; } = string.Empty;

    [TrackState] public decimal ProductUnitPrice { get; set; }
 
    public ProductEditContext() : base() { }

    protected override Product MapEditFieldsToRecord()
        => this.BaseRecord with
        {
            ProductName = this.ProductName,
            ProductUnitPrice = this.ProductUnitPrice,
            ProductCode = this.ProductCode
        };

    protected override void MapToContext(Product record)
    {
        this.Uid = record.Uid;
        this.ProductName = record.ProductName;
        this.ProductCode = record.ProductCode;
        this.ProductUnitPrice = record.ProductUnitPrice;
    }

    protected override Product MapEditFieldsAndStateToRecord()
        => MapEditFieldsToRecord() with { EntityState = this.EntityState };
}
