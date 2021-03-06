/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public class BaseEntityService<TEntity>
    :IEntityService<TEntity>
    where TEntity : class, IEntity
{
}
