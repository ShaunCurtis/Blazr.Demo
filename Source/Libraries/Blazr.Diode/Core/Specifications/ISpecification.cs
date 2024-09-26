/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode.Core;

public interface ISpecification<T>
{
    public bool IsSatisfiedBy(T entity);
}
