/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public readonly struct ApplicationConstants
{
    public readonly struct Common
    {
        public const string Uid = "Uid";
        public const string Date = "Date";
    }

    public readonly struct Invoice
    {
        //Invoice Fields
        public const string InvoiceNumber = "InvoiceNumber";
        public const string InvoiceDate = "InvoiceDate";
        public const string CustomerUid = "CustomerUid";
        public const string InvoicePrice = "InvoicePrice";

        //Invoice Filters
        public const string FilterByCustomerUid = "FilterByCustomerUid";
        public const string FilterByInvoiceMonth = "FilterByInvoiceMonth";
    }

    public readonly struct InvoiceItem
    {
        //InvoiceItem Fields
        public const string InvoiceUid = "InvoiceUid";
        public const string ProductUid = "ProductUid";
        public const string ItemQuantity = "ItemQuantity";
        public const string ItemUnitPrice = "ItemUnitPrice";

        //InvoiceItem Filters
        public const string FilterByInvoiceUid = "FilterByInvoiceUid";
        public const string FilterByProductUid = "FilterByProductUid";
    }

    public readonly struct Customer
    {
        //Customer Fields
        public const string CustomerName = "CustomerName";
    }

    public readonly struct Product
    {
        //Product Fields
        public const string ProductName = "ProductName";
        public const string ProductCode = "ProductCode";
        public const string ProductUnitPrice = "ProductUnitPrice";


        public const string FilterByManufacturerName = "FilterByManufacturerName";
        
    }
}
