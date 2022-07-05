/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class FilteredListQuery<TRecord>
    :IFilteredListQuery<TRecord>
{

    public ListProviderRequest Request { get; private set; }

    public Func<TRecord, bool>? FilterExpression { get; private set; } = null;

    public Guid TransactionId => Guid.NewGuid();

    public FilteredListQuery(ListProviderRequest request, Func<TRecord, bool>? filter)
    {
        Request = request;
        FilterExpression = filter;
    }
}
