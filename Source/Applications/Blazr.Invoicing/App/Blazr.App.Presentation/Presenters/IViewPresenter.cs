/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public interface IViewPresenter<TRecord>
    where TRecord : class, new()
{
    public IDataResult LastDataResult { get; }
    public TRecord Item { get; }

    public Task LoadAsync(object id);
}
