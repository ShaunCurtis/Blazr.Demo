/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public sealed class InMemoryInvoiceDbContext
    : DbContext
{
    public DbSet<User> User { get; set; } = default!;
    public DbSet<Customer> Customer { get; set; } = default!;
    public DbSet<Product> Product { get; set; } = default!;
    public DbSet<Invoice> InvoiceView { get; set; } = default!;
    public DbSet<InvoiceItem> InvoiceItemView { get; set; } = default!;
    public DbSet<CustomerFkItem> CustomerFK { get; set; }
    public DbSet<ProductFkItem> ProductFK { get; set; }

    internal DbSet<DboInvoice> DboInvoice { get; set; } = default!;
    internal DbSet<DboInvoiceItem> DboInvoiceItem { get; set; } = default!;

    public InMemoryInvoiceDbContext(DbContextOptions<InMemoryInvoiceDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("User");
        modelBuilder.Entity<Customer>().ToTable("Customer");
        modelBuilder.Entity<Product>().ToTable("Product");
        modelBuilder.Entity<DboInvoice>().ToTable("Invoice");
        modelBuilder.Entity<DboInvoiceItem>().ToTable("InvoiceItem");

        modelBuilder.Entity<Invoice>()
            .ToInMemoryQuery(()
                => from i in this.DboInvoice
                   join c in this.Customer! on i.CustomerUid equals c.Uid
                   select new Invoice
                   {
                       Uid = i.Uid,
                       CustomerUid = i.CustomerUid,
                       CustomerName = c.CustomerName,
                       InvoiceDate = i.InvoiceDate,
                       InvoiceNumber = i.InvoiceNumber,
                       InvoicePrice = i.InvoicePrice,
                   }).HasKey(x => x.Uid);

        modelBuilder.Entity<InvoiceItem>()
            .ToInMemoryQuery(()
                => from i in this.DboInvoiceItem
                   join p in this.Product! on i.ProductUid equals p.Uid
                   join iv in this.DboInvoice! on i.InvoiceUid equals iv.Uid
                   select new InvoiceItem
                   {
                       Uid = i.Uid,
                       InvoiceUid = i.InvoiceUid,
                       InvoiceNumber = iv.InvoiceNumber,
                       ProductUid = i.ProductUid,
                       ProductName = p.ProductName,
                       ProductCode = p.ProductCode,
                       ItemQuantity = i.ItemQuantity,
                       ItemUnitPrice = i.ItemUnitPrice,
                   }).HasKey(x => x.Uid);

        modelBuilder.Entity<CustomerFkItem>()
            .ToInMemoryQuery(()
                => from c in this.Customer
                   select new CustomerFkItem
                   {
                       Uid = c.Uid,
                       Name = c.CustomerName,
                   }).HasKey(x => x.Uid);

        modelBuilder.Entity<ProductFkItem>()
            .ToInMemoryQuery(()
                => from p in this.Product
                   select new ProductFkItem
                   {
                       Uid = p.Uid,
                       Name = p.ProductName,
                       ProductCode = p.ProductCode,
                       ItemUnitPrice = p.ProductUnitPrice
                   }).HasKey(x => x.Uid);
    }
}
