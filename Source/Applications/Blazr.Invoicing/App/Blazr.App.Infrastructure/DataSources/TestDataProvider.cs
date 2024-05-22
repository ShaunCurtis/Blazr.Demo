
using static Blazr.App.Core.AppDictionary;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

public sealed class TestDataProvider
{
    public IEnumerable<DboCustomer> Customers => _customers.AsEnumerable();
    public IEnumerable<DboInvoice> Invoices => _invoices.AsEnumerable();
    public IEnumerable<DboInvoiceItem> InvoiceItems => _invoiceItems.AsEnumerable();

    private List<DboCustomer> _customers = new List<DboCustomer>();
    private List<DboInvoice> _invoices = new List<DboInvoice>();
    private List<DboInvoiceItem> _invoiceItems = new List<DboInvoiceItem>();

    public TestDataProvider()
    {
        this.Load();
    }

    private void Load()
    {
        _customers = new();

        var id = Guid.NewGuid();
        DboCustomer customer = new()
        {
            CustomerID = id,
            CustomerName = "EasyJet"
        };
        _customers.Add(customer);

        {
            var _id = Guid.NewGuid();
            _invoices.Add(new()
            {
                InvoiceID = _id,
                CustomerID = id,
                Date = DateTime.Now.AddDays(-3),
                TotalAmount = 50
            });
            _invoiceItems.Add(new()
            {
                InvoiceItemID = Guid.NewGuid(),
                InvoiceID = _id,
                Description = "Airbus A321",
                Amount = 15
            });
            _invoiceItems.Add(new()
            {
                InvoiceItemID = Guid.NewGuid(),
                InvoiceID = _id,
                Description = "Airbus A350",
                Amount = 35
            });
        }

        id = Guid.NewGuid();
        customer = new()
        {
            CustomerID = id,
            CustomerName = "RyanAir"
        };
        _customers.Add(customer);

        {
            var _id = Guid.NewGuid();
            _invoices.Add(new()
            {
                InvoiceID = _id,
                CustomerID = id,
                Date = DateTime.Now.AddDays(-2),
                TotalAmount = 27
            });
            _invoiceItems.Add(new()
            {
                InvoiceItemID = Guid.NewGuid(),
                InvoiceID = _id,
                Description = "Airbus A319",
                Amount = 12
            });
            _invoiceItems.Add(new()
            {
                InvoiceItemID = Guid.NewGuid(),
                InvoiceID = _id,
                Description = "Airbus A321",
                Amount = 15
            });
        }

        id = Guid.NewGuid();
        customer = new()
        {
            CustomerID = id,
            CustomerName = "Air France"
        };
        _customers.Add(customer);

        {
            var _id = Guid.NewGuid();
            _invoices.Add(new()
            {
                InvoiceID = _id,
                CustomerID = id,
                Date = DateTime.Now.AddDays(-1),
                TotalAmount = 60
            });
            _invoiceItems.Add(new()
            {
                InvoiceItemID = Guid.NewGuid(),
                InvoiceID = _id,
                Description = "Airbus A330",
                Amount = 25
            });
            _invoiceItems.Add(new()
            {
                InvoiceItemID = Guid.NewGuid(),
                InvoiceID = _id,
                Description = "Airbus A350",
                Amount = 35
            });
        }
    }

    public void LoadDbContext<TDbContext>(IDbContextFactory<TDbContext> factory) where TDbContext : DbContext
    {
        using var dbContext = factory.CreateDbContext();

        var dboCustomers = dbContext.Set<DboCustomer>();
        var dboInvoices = dbContext.Set<DboInvoice>();
        var dboInvoiceItems = dbContext.Set<DboInvoiceItem>();

        // Check if we already have a full data set
        // If not clear down any existing data and start again
        if (dboCustomers.Count() == 0)
            dbContext.AddRange(_customers);

        if (dboInvoices.Count() == 0)
            dbContext.AddRange(_invoices);

        if (dboInvoiceItems.Count() == 0)
            dbContext.AddRange(_invoiceItems);

        dbContext.SaveChanges();
    }

    private static TestDataProvider? _provider;

    public static TestDataProvider Instance()
    {
        if (_provider is null)
            _provider = new TestDataProvider();

        return _provider;
    }
}