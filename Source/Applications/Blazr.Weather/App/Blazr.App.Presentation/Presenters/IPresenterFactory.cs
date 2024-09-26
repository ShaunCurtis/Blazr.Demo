/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public interface IPresenterFactory
{
    public ValueTask<IEditPresenter<TRecord, TKey, TEditContext>> CreateEditPresenterAsync<TRecord, TKey, TEditContext>(TKey id, bool isNew)
        where TRecord : class, new()
        where TEditContext : IRecordEditContext<TRecord>, new();

    public ValueTask<IDataGridPresenter> CreateDataGridPresenterAsync();

    public ValueTask<IViewPresenter<TRecord, TKey>> CreateViewPresenterAsync<TRecord, TKey>(TKey id)
        where TRecord : class, new();

    public ValueTask<GuidLookUpPresenter<TLookupItem>> CreateGuidLookupPresenterAsync<TLookupItem>()
        where TLookupItem : class, IGuidLookUpItem, new();
}
