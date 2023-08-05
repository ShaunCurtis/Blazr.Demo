/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet;

public interface IOwsResult<TEntity>
    where TEntity : class, IOwsEntity
{
    public TEntity? Entity { get; }
    public bool Successful { get; }
    public string? Message { get; }
}