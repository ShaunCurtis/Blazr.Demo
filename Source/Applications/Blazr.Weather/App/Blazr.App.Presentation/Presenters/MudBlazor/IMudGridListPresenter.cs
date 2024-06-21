/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using MudBlazor;

namespace Blazr.App.Presentation.MudBlazor;

public interface IMudGridListPresenter<TRecord>
    where TRecord : class, new()
{
    public IDataResult LastDataResult { get; }
    public int DefaultPageSize { get; set; }
    public List<FilterDefinition>? Filters { get; set; }

    public Task<GridData<TRecord>> GetItemsAsync(GridState<TRecord> request);
}
