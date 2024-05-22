/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Infrastructure;

public interface IDboEntityMap<TDboEntity, TDomainEntity>
{
    public TDomainEntity MapTo(TDboEntity item);
    public TDboEntity MapTo(TDomainEntity item);
}
