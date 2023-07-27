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
    internal DbSet<DboCustomer> DboCustomer { get; set; } = default!;
    internal DbSet<DboProduct> DboProduct { get; set; } = default!;
    internal DbSet<DboUser> DboUser { get; set; } = default!;

    public InMemoryInvoiceDbContext(DbContextOptions<InMemoryInvoiceDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DboUser>().ToTable("User");
        modelBuilder.Entity<DboCustomer>().ToTable("Customer");
        modelBuilder.Entity<DboProduct>().ToTable("Product");
        modelBuilder.Entity<DboInvoice>().ToTable("Invoice");
        modelBuilder.Entity<DboInvoiceItem>().ToTable("InvoiceItem");

        modelBuilder.Entity<User>()
            .ToInMemoryQuery(()
            => from u in this.DboUser
               select new User
               {
                   EntityState = new(StateCodes.Existing),
                   UserUid = new(u.Uid),
                   UserName = u.UserName,
                   Roles = u.Roles,
               }).HasNoKey();

        modelBuilder.Entity<Customer>()
            .ToInMemoryQuery(()
            => from c in this.DboCustomer
               select new Customer
               {
                   EntityState = new(StateCodes.Existing),
                   CustomerUid = new(c.Uid),
                   CustomerName = c.CustomerName,
               }).HasNoKey();

        modelBuilder.Entity<Product>()
            .ToInMemoryQuery(()
            => from p in this.DboProduct
               select new Product
               {
                   EntityState = new(StateCodes.Existing),
                   ProductUid = new(p.Uid),
                   ProductName = p.ProductName,
                   ProductCode = p.ProductCode,
                   ProductUnitPrice = p.ProductUnitPrice,
               }).HasNoKey();

        modelBuilder.Entity<Invoice>()
            .ToInMemoryQuery(()
                => from i in this.DboInvoice
                   join c in this.DboCustomer! on i.CustomerUid equals c.Uid
                   select new Invoice
                   {
                       InvoiceUid = new( i.Uid),
                       CustomerUid = new(i.CustomerUid),
                       CustomerName = c.CustomerName,
                       InvoiceDate = i.InvoiceDate,
                       InvoiceNumber = i.InvoiceNumber,
                       InvoicePrice = i.InvoicePrice,
                   }).HasNoKey();

        modelBuilder.Entity<InvoiceItem>()
            .ToInMemoryQuery(()
                => from i in this.DboInvoiceItem
                   join p in this.DboProduct! on i.ProductUid equals p.Uid
                   join iv in this.DboInvoice! on i.InvoiceUid equals iv.Uid
                   select new InvoiceItem
                   {
                       InvoiceItemUid = new(i.Uid),
                       InvoiceUid = new( i.InvoiceUid),
                       InvoiceNumber = iv.InvoiceNumber,
                       ProductUid = new(i.ProductUid),
                       ProductName = p.ProductName,
                       ProductCode = p.ProductCode,
                       ItemQuantity = i.ItemQuantity,
                       ItemUnitPrice = i.ItemUnitPrice,
                   }).HasNoKey();

        modelBuilder.Entity<CustomerFkItem>()
            .ToInMemoryQuery(()
                => from c in this.DboCustomer
                   select new CustomerFkItem
                   {
                       Uid = c.Uid,
                       Name = c.CustomerName,
                   }).HasNoKey();

        modelBuilder.Entity<ProductFkItem>()
            .ToInMemoryQuery(()
                => from p in this.DboProduct
                   select new ProductFkItem
                   {
                       Uid = p.Uid,
                       Name = p.ProductName,
                       ProductCode = p.ProductCode,
                       ItemUnitPrice = p.ProductUnitPrice
                   }).HasNoKey();
    }
}
