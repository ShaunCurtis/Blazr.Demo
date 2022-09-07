/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public interface IReadService<TRecord, TEntity>
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    public TRecord? Record { get; }

    public string? Message { get; }

    public void SetServices(IServiceProvider services);

    public ValueTask<bool> LoadRecordAsync(Guid Id);
}

