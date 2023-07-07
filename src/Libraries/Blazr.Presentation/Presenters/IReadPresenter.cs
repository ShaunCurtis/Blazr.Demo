/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Presentation;

public interface IReadPresenter<TRecord>
    where TRecord : class, new()
{
    public TRecord Item { get;}

    public ValueTask LoadAsync(Guid id);
}