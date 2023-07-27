/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Infrastructure;

public interface IDboEntityMap<TDboEntity, TDomainEntity>
{
    public TDomainEntity Map(TDboEntity item);
    public TDboEntity Map(TDomainEntity item);
}
