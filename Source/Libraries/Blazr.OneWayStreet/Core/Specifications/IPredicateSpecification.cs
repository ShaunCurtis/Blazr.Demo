/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Core;

public interface IPredicateSpecification<T>
{
    public bool IsSatisfiedBy(T entity);

    public IQueryable<T> AsQueryAble(IQueryable<T> query);

    public IEnumerable<T> AsEnumerable(IEnumerable<T> query);
}
