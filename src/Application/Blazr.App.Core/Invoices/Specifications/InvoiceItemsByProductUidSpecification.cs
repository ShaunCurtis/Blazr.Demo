/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class InvoiceItemsByProductUidSpecification : PredicateSpecification<InvoiceItem>
{
    private readonly Guid _productUid;

    public InvoiceItemsByProductUidSpecification(Guid uid)
        => _productUid = uid;

    public InvoiceItemsByProductUidSpecification(FilterDefinition filter)
        => Guid.TryParse(filter.FilterData, out _productUid);

    public override Expression<Func<InvoiceItem, bool>> Expression
        => item => item.ProductUid == new ProductUid(_productUid);
}
