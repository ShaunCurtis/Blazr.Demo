/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Presentation;

public interface IBlazrEditPresenter<TRecord, TEditContext>
    where TRecord : class, IStateEntity, IEntity, new()
    where TEditContext : class, IBlazrRecordEditContext<TRecord>, new()
{
    public IDataResult LastResult { get; }

    public TEditContext RecordContext { get; }

    public EditContext EditContext { get; }

    public ValueTask LoadAsync(Guid id);

    public ValueTask ResetItemAsync();

    public ValueTask SaveItemAsync();
}