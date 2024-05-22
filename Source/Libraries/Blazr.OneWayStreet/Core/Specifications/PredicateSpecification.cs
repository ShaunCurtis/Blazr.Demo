/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Core;

public abstract class PredicateSpecification<T> : IPredicateSpecification<T>
{
    protected Func<T, bool> predicate => this.Expression.Compile();

    public abstract Expression<Func<T, bool>> Expression { get; }

    public bool IsSatisfiedBy(T entity)
        => predicate(entity);

    public IQueryable<T> AsQueryAble(IQueryable<T> query)
        => query.Where(predicate).AsQueryable();

    public IEnumerable<T> AsEnumerable(IEnumerable<T> query)
        => query.Where(predicate);
}

