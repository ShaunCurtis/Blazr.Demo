/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public interface IPresenterFactory
{
    public ValueTask<IEditPresenter<TRecord, TIdentity, TEditContext>> CreateEditPresenterAsync<TRecord, TIdentity, TEditContext>(TIdentity id, bool isNew)
        where TRecord : class, new()
        where TIdentity : IEntityKey
        where TEditContext : IRecordEditContext<TRecord>, new();

    public ValueTask<IDataGridPresenter> CreateDataGridPresenterAsync();

    public ValueTask<IViewPresenter<TRecord, TIdentity>> CreateViewPresenterAsync<TRecord, TIdentity>(TIdentity id)
        where TRecord : class, new()
        where TIdentity : IEntityKey;

    public ValueTask<GuidLookUpPresenter<TLookupItem>> CreateGuidLookupPresenterAsync<TLookupItem>()
        where TLookupItem : class, IGuidLookUpItem, new();
}
