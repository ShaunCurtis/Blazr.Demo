/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.Data;

public abstract class ListQueryHandlerBase<TAction, TRecord>
    : ICQSHandler<TAction, ValueTask<ListProviderResult<TRecord>>>
    where TAction : ICQSRequest<ValueTask<ListProviderResult<TRecord>>>
{
    protected IEnumerable<TRecord> items = Enumerable.Empty<TRecord>();
    protected int count = 0;
    protected bool success = false;
    protected string message = string.Empty;

    protected readonly RecordListQuery<TRecord> listQuery;

    public ListQueryHandlerBase(RecordListQuery<TRecord> query)
    {
        this.listQuery = query;
    }

    public async ValueTask<ListProviderResult<TRecord>> ExecuteAsync()
        => await _executeAsync();

    private async ValueTask<ListProviderResult<TRecord>> _executeAsync()
    {
        if (this.listQuery is null)
        {
            this.success = false;
            this.message = "No ListQuery provided.";
            return new ListProviderResult<TRecord>(this.items, this.count, this.success, this.message);
        }

        await this.GetItemsAsync();
        if (this.success)
            await this.GetCountAsync();

        return new ListProviderResult<TRecord>(this.items, this.count, this.success, this.message);
    }

    protected abstract ValueTask GetItemsAsync();

    protected abstract ValueTask GetCountAsync();
}
