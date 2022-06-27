/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Demo.Data;

public abstract class LookupListQueryHandlerBase<TAction, TRecord>
    : ICQSHandler<TAction, ValueTask<LookupListProviderResult>>
    where TAction : ICQSRequest<ValueTask<LookupListProviderResult>>
{
    protected IEnumerable<TRecord> items = Enumerable.Empty<TRecord>();
    protected int count = 0;

    protected readonly DbContext dbContext;
    protected readonly LookupListQuery<TRecord> listQuery;

    public LookupListQueryHandlerBase(DbContext dbContext, LookupListQuery<TRecord> query)
    {
        this.dbContext = dbContext;
        this.listQuery = query;
    }

    public abstract ValueTask<LookupListProviderResult> ExecuteAsync();
}
