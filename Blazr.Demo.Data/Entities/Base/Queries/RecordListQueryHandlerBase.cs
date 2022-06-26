/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.Data;

public abstract class RecordListQueryHandlerBase<TAction, TRecord>
    : IRequestHandler<TAction, ValueTask<ListProviderResult<TRecord>>>
    where TAction : IHandlerRequest<ValueTask<ListProviderResult<TRecord>>>
{
    protected IEnumerable<TRecord> items = Enumerable.Empty<TRecord>();
    protected int count = 0;

    protected readonly DbContext dbContext;
    protected readonly RecordListQuery<TRecord> listQuery;

    public RecordListQueryHandlerBase(DbContext dbContext, RecordListQuery<TRecord> query)
    {
        this.dbContext = dbContext;
        this.listQuery = query;
    }

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync()
        => await _executeAsync();

    private async ValueTask<ListProviderResult<TRecord>> _executeAsync()
    {
        if (this.listQuery is null)
            return new ListProviderResult<TRecord>(new List<TRecord>(), 0);
        if (await this.GetItemsAsync())
            await this.GetCountAsync();
        return new ListProviderResult<TRecord>(this.items, this.count);
    }

    protected abstract ValueTask<bool> GetItemsAsync();

    protected abstract ValueTask<bool> GetCountAsync();
}
