/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

/// <summary>
/// A class to build a fixed data set for testing
/// </summary>
public sealed class InvoiceTestDataProvider
{
    public IEnumerable<DboUser> Users => _users ?? Enumerable.Empty<DboUser>();
    public IEnumerable<DboProduct> Products => _products ?? Enumerable.Empty<DboProduct>();
    public IEnumerable<DboCustomer> Customers => _customers ?? Enumerable.Empty<DboCustomer>();
    internal IEnumerable<DboInvoice> Invoices => _invoices ?? Enumerable.Empty<DboInvoice>();
    internal IEnumerable<DboInvoiceItem> InvoiceItems => _invoiceItems ?? Enumerable.Empty<DboInvoiceItem>();

    private List<DboProduct>? _products;
    private List<DboCustomer>? _customers;
    private List<DboInvoice>? _invoices;
    private List<DboInvoiceItem>? _invoiceItems;
    private List<DboUser>? _users;

    private InvoiceTestDataProvider()
        => this.Load();

    public void LoadDbContext<TDbContext>(IDbContextFactory<TDbContext> factory) where TDbContext : DbContext
    {
        using var dbContext = factory.CreateDbContext();

        var products = dbContext.Set<Product>();

        // Check if we already have a full data set
        // If not clear down any existing data and start again
        if (products.Count() == 0)
        {
            dbContext.AddRange(this.Products);
            dbContext.SaveChanges();
        }

        var customers = dbContext.Set<Customer>();

        // Check if we already have a full data set
        // If not clear down any existing data and start again
        if (customers.Count() == 0)
        {
            dbContext.AddRange(this.Customers);
            dbContext.SaveChanges();
        }

        var invoices = dbContext.Set<DboInvoice>();

        // Check if we already have a full data set
        // If not clear down any existing data and start again
        if (invoices.Count() == 0)
        {
            dbContext.AddRange(this.Invoices);
            dbContext.SaveChanges();
        }

        var invoiceItems = dbContext.Set<DboInvoiceItem>();

        // Check if we already have a full data set
        // If not clear down any existing data and start again
        if (invoiceItems.Count() == 0)
        {
            dbContext.AddRange(this.InvoiceItems);
            dbContext.SaveChanges();
        }

        var users = dbContext.Set<User>();

        // Check if we already have a full data set
        // If not clear down any existing data and start again
        if (users.Count() == 0)
        {
            dbContext.AddRange(this.Users);
            dbContext.SaveChanges();
        }
    }

    public void Load()
    {
        LoadProducts();
        LoadCustomers();
        LoadInvoices();
        LoadInvoiceItems();
        LoadUsers();
    }

    private void LoadUsers()
    {
        var users = new List<DboUser>()
        {
            new() { Uid=Guid.Parse("10000000-0000-0000-0000-000000000001"), UserName="Visitor-1", Roles="VisitorRole"},
            new() { Uid=Guid.Parse("20000000-0000-0000-0000-000000000001"), UserName="User-1", Roles="UserRole"},
            new() { Uid=Guid.Parse("20000000-0000-0000-0000-000000000002"), UserName="User-2", Roles="UserRole"},
            new() { Uid=Guid.Parse("30000000-0000-0000-0000-000000000001"), UserName="Admin-1", Roles="AdminRole"},
        };
        _users = users;
    }

    private void LoadProducts()
    {
        var products = new List<DboProduct>()
        {
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000001"), ProductCode="SKU100", ProductName="Boeing 707", ProductUnitPrice=12.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000002"), ProductCode="SKU101", ProductName="Boeing 727", ProductUnitPrice=13.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000003"), ProductCode="SKU102", ProductName="Boeing 737", ProductUnitPrice=14.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000004"), ProductCode="SKU103", ProductName="Boeing 747", ProductUnitPrice=19.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000005"), ProductCode="SKU104", ProductName="Boeing 757", ProductUnitPrice=25.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000006"), ProductCode="SKU105", ProductName="Boeing 767", ProductUnitPrice=26.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000007"), ProductCode="SKU106", ProductName="Boeing 777", ProductUnitPrice=31.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000008"), ProductCode="SKU107", ProductName="Boeing 787", ProductUnitPrice=32.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000009"), ProductCode="SKU110", ProductName="Airbus A300", ProductUnitPrice=12.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000010"), ProductCode="SKU111", ProductName="Airbus A310", ProductUnitPrice=13.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000011"), ProductCode="SKU112", ProductName="Airbus A319", ProductUnitPrice=14.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000012"), ProductCode="SKU113", ProductName="Airbus A320", ProductUnitPrice=15.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000013"), ProductCode="SKU114", ProductName="Airbus A321", ProductUnitPrice=16.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000014"), ProductCode="SKU115", ProductName="Airbus A330", ProductUnitPrice=17.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000015"), ProductCode="SKU116", ProductName="Airbus A340", ProductUnitPrice=22.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000016"), ProductCode="SKU117", ProductName="Airbus A350", ProductUnitPrice=23.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000017"), ProductCode="SKU118", ProductName="Airbus A380", ProductUnitPrice=25.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000018"), ProductCode="SKU119", ProductName="Airbus A220", ProductUnitPrice=37.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000019"), ProductCode="SKU220", ProductName="Fokker F27", ProductUnitPrice=2.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000020"), ProductCode="SKU221", ProductName="Fokker F28", ProductUnitPrice=3.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000021"), ProductCode="SKU321", ProductName="BAE 146", ProductUnitPrice=4.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000022"), ProductCode="SKU323", ProductName="BAE ATR", ProductUnitPrice=3.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000023"), ProductCode="SKU401", ProductName="Tupolev 204", ProductUnitPrice=7.50m },
            new() { Uid=Guid.Parse("11111111-0000-0000-0000-000000000024"), ProductCode="SKU402", ProductName="Tupolev 214", ProductUnitPrice=4.50m },
        };
        _products = products;
    }

    private void LoadCustomers()
    {
        var customers = new List<DboCustomer>()
        {
            new() { Uid=Guid.Parse("11111111-1111-0000-0000-000000000001"), CustomerName="EasyJet"},
            new() { Uid=Guid.Parse("11111111-1111-0000-0000-000000000002"), CustomerName="RyanAir"},
            new() { Uid=Guid.Parse("11111111-1111-0000-0000-000000000003"), CustomerName="Air France"},
            new() { Uid=Guid.Parse("11111111-1111-0000-0000-000000000004"), CustomerName="TAP"},
        };
        _customers = customers;
    }

    private void LoadInvoices()
    {
        var invoices = new List<DboInvoice>()
        {
            new() { Uid=Guid.Parse("11111111-1111-1111-0000-000000000001"), CustomerUid = _customers![Random.Shared.Next(Customers.Count())].Uid, StateCode=1, InvoiceDate = DateOnly.FromDateTime(DateTime.Now), InvoiceNumber="1001", InvoicePrice=1000m},
            new() { Uid=Guid.Parse("11111111-1111-1111-0000-000000000002"), CustomerUid = _customers![Random.Shared.Next(Customers.Count())].Uid, StateCode=2, InvoiceDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10)), InvoiceNumber="1002", InvoicePrice=2000m},
            new() { Uid=Guid.Parse("11111111-1111-1111-0000-000000000003"), CustomerUid = _customers![Random.Shared.Next(Customers.Count())].Uid, StateCode=3, InvoiceDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30)), InvoiceNumber="1003", InvoicePrice=3000m},
        };
        _invoices = invoices;
    }

    private void LoadInvoiceItems()
    {
        var invoiceItems = new List<DboInvoiceItem>();

        foreach (var invoice in this.Invoices)
        {
            var product = _products![Random.Shared.Next(_products.Count())];
            invoiceItems.Add(new() { InvoiceUid = invoice.Uid, ProductUid = product.Uid, ItemQuantity = Random.Shared.Next(1, 4), ItemUnitPrice = product.ProductUnitPrice });
            product = _products![Random.Shared.Next(_products.Count())];
            invoiceItems.Add(new() { InvoiceUid = invoice.Uid, ProductUid = product.Uid, ItemQuantity = Random.Shared.Next(1, 4), ItemUnitPrice = product.ProductUnitPrice });
        }

        _invoiceItems = invoiceItems;
    }

    private static InvoiceTestDataProvider? _provider;

    public InvoiceItem GetNewInvoiceItem(Guid invoiceUid)
    {
        var invoice = _invoices!.First(item => item.Uid == invoiceUid);
        var product = _products![Random.Shared.Next(_products.Count())];
        return new()
        {
            EntityState = new(StateCodes.New),
            InvoiceUid = new(invoiceUid),
            InvoiceNumber = invoice.InvoiceNumber,
            ProductUid = new(product.Uid),
            ProductName = product.ProductName,
            ProductCode = product.ProductCode,
            ItemQuantity = Random.Shared.Next(4),
            ItemUnitPrice = product.ProductUnitPrice
        };
    }

    public Invoice GetNewInvoice()
    {
        return new()
        {
            EntityState = new(StateCodes.New),
            InvoiceUid = new( Guid.NewGuid()),
            CustomerUid = new( _customers![Random.Shared.Next(Customers.Count())].Uid),
            InvoiceDate = DateOnly.FromDateTime(DateTime.Now),
            InvoiceNumber = "1006",
            InvoicePrice = 1000m
        };
    }

    public InvoiceUid TestInvoiceUid
        => new(_invoices?.First().Uid ?? Guid.Empty);

    public Product TestProduct
        => _products?.First().FromDbo() ?? new();

    public Guid TestProductUid
        => _products?.First().Uid ?? Guid.Empty;

    public Product FirstManufacturersProduct(string manufacturer) 
        => _products!
         .Where(item => item.ProductName
         .Contains(manufacturer))
         .OrderBy(item => item.ProductName)
         .FirstOrDefault()!.FromDbo();

    public Product FirstProduct 
        => _products!.OrderBy(item => item.ProductName).FirstOrDefault()!.FromDbo();

    public Product RandomProduct 
        => _products!.Skip(Random.Shared.Next(0, _products!.Count() - 1)).First().FromDbo();

    public Customer TestCustomer
        => _customers?.First().FromDbo() ?? new();

    public Guid TestCustomerUid
        => _invoices?.First().CustomerUid ?? Guid.Empty;

    public int CustomerInvoiceCount(Guid Uid)
    => _invoices?.Where(item => item.CustomerUid == Uid).Count() ?? 0;

    public int InvoiceCount
        => _invoices?.Count() ?? 0;

    public static InvoiceTestDataProvider Instance()
    {
        if (_provider is null)
            _provider = new InvoiceTestDataProvider();

        return _provider;
    }
}
