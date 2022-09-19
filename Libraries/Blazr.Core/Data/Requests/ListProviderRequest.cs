/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public readonly struct ListProviderRequest<TRecord>
    where TRecord : class, new()
{
    public int StartIndex { get; }

    public int PageSize { get; }

    public bool SortDescending { get; } = false;

    public Expression<Func<TRecord, bool>>? FilterExpression { get; }

    public Expression<Func<TRecord, object>>? SortExpression { get; }

    public CancellationToken CancellationToken { get; }

    public ListProviderRequest()
    { 
        this.StartIndex = 0;
        this.PageSize = 10000;
        this.CancellationToken = new CancellationToken();
        this.SortExpression = null;
        this.FilterExpression = null;
    }

    public ListProviderRequest(ListState<TRecord> state, CancellationToken? cancellationToken = null)
    {
        this.StartIndex = state.StartIndex;
        this.PageSize = state.PageSize;
        this.CancellationToken = cancellationToken ?? new CancellationToken();
        this.SortDescending = state.SortDescending;
        this.SortExpression = state.SortExpression;
        this.FilterExpression = state.FilterExpression;
    }
}
