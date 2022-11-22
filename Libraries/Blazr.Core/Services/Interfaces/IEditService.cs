/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public interface IEditService<TRecord, TEditRecord, TEntity>
    where TRecord : class, new()
    where TEditRecord : class, IRecordEditContext<TRecord>, new()
    where TEntity : class, IEntity
{
    public TEditRecord EditModel { get; }

    public string? Message { get; }

    public void SetServices(IServiceProvider services);

    public ValueTask<bool> LoadRecordAsync(Guid Id);

    public ValueTask<bool> GetNewEditRecordAsync(TRecord? record);

    public ValueTask<bool> AddRecordAsync();

    public ValueTask<bool> UpdateRecordAsync();

    public ValueTask<bool> DeleteRecordAsync();
}
