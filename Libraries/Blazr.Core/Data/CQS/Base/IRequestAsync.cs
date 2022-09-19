/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface IRequestAsync<out TResult>
{
    public Guid TransactionId { get;}
    
    public CancellationToken CancellationToken { get; }
}
