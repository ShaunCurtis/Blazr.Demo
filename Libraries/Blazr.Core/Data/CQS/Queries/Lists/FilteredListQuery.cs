/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public class CustomListQuery<TRecord>
    :ICustomListQuery<TRecord>
{
    public ListProviderRequest<TRecord> Request { get; private set; }

    public Func<TRecord, bool>? FilterExpression { get; private set; } = null;

    public Guid TransactionId => Guid.NewGuid();

    public CustomListQuery(ListProviderRequest request, Func<TRecord, bool>? filter)
    {
        Request = request;
        FilterExpression = filter;
    }
}
