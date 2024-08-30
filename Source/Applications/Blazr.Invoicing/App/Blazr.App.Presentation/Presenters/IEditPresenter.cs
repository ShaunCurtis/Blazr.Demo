/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;
public interface IEditPresenter<TRecord, TIdentity, TEditContext>
    where TRecord : class, new()
    where TIdentity : IEntityKey
    where TEditContext : IRecordEditContext<TRecord>, new()
{
    public IDataResult LastDataResult { get; }
    public EditContext EditContext { get; }
    public TEditContext RecordEditContext { get; }
    public bool IsNew { get; }

    public Task LoadAsync(TIdentity id, bool isNew);

    public Task<IDataResult> SaveItemAsync();
}
