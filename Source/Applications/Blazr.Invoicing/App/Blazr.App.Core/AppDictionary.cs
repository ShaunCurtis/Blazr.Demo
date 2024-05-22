/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core
{
    public static class AppDictionary
    {
        public static class Common
        {
        }

        public static class Customer
        {
            public const string CustomerId = "CustomerId";
            public const string CustomerID = "CustomerID";
            public const string CustomerName = "CustomerName";
        }

        public static class Invoice
        {
            public const string InvoiceId = "InvoiceId";
            public const string InvoiceID = "InvoiceID";
            public const string TotalAmount = "TotalAmount";
            public const string InvoiceFilterByCustomerSpecification = "InvoiceFilterByCustomerSpecification";
        }

        public static class InvoiceItem
        {
            public const string InvoiceItemId = "InvoiceItemId";
            public const string InvoiceItemID = "InvoiceItemID";
            public const string Amount = "Amount";
            public const string InvoiceItemFilterByInvoiceSpecification = "InvoiceItemFilterByInvoiceSpecification";
        }
    }
}
