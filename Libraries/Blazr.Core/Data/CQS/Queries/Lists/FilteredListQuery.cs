/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class FilteredListQuery<TRecord>
    :IFilteredListQuery<TRecord>
    where TRecord : class, new()
{
    public ListProviderRequest<TRecord> Request { get; private set; }

    public Guid TransactionId => Guid.NewGuid();

    public FilteredListQuery(ListProviderRequest<TRecord> request)
    {
        Request = request;
    }
}
