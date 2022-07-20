/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public interface IListService<TRecord, TEntity>
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    public int PageSize { get;}

    public int StartIndex { get; }

    public int ListCount { get; }

    public IEnumerable<TRecord>? Records { get; }

    public string? Message { get; }

    public bool IsPaging => (PageSize > 0);

    public bool HasList => this.Records is not null;

    public void SetServices(IServiceProvider services);

    public ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(int startRecord, int pageSize);

    public ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(ListProviderRequest<TRecord> request);

    public ValueTask<ItemsProviderResult<TRecord>> GetRecordsAsync(ItemsProviderRequest request);

    public ValueTask<ListProviderResult<TRecord>> GetRecordsAsync(IFilteredListQuery<TRecord> query);
}

