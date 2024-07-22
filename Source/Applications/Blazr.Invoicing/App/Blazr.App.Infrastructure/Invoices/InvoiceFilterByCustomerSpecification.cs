/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.Linq.Expressions;

namespace Blazr.App.Infrastructure;

public class InvoiceFilterByCustomerSpecification : PredicateSpecification<DvoInvoice>
{
    private CustomerId _customerId = new(Guid.Empty);

    public InvoiceFilterByCustomerSpecification()
    { }

    public InvoiceFilterByCustomerSpecification(FilterDefinition filter)
    {
        filter.TryFromJson<CustomerId>(out CustomerId? _id);
        _customerId = _id ?? new(Guid.Empty);
    }

    public override Expression<Func<DvoInvoice, bool>> Expression
        => item => item.CustomerID == _customerId.Value;
}
