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
    public RecordList<TRecord> Records { get; }

    public string? Message { get; }

    public void SetServices(IServiceProvider services);

    public ValueTask<bool> GetRecordsAsync(ListProviderRequest<TRecord> request);

    public ValueTask<bool> GetRecordsAsync(ListQuery<TRecord> query);
}

