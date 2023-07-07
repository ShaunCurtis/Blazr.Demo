/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class InvoicesByCustomerUidSpecification : PredicateSpecification<Invoice>
{
    private readonly Guid _customerUid;

    public InvoicesByCustomerUidSpecification(Guid customerUid)
        => _customerUid = customerUid;
    
    public InvoicesByCustomerUidSpecification(FilterDefinition filter)
        => Guid.TryParse(filter.FilterData, out _customerUid);

    public override Expression<Func<Invoice, bool>> Expression
        => invoice => invoice.CustomerUid == _customerUid;
}
