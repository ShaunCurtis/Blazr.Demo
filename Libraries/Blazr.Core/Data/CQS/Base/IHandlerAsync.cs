/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface IHandlerAsync<in TRequest, out TResult>
    where TRequest : IRequestAsync<TResult>
{
    TResult ExecuteAsync(TRequest request);
}
