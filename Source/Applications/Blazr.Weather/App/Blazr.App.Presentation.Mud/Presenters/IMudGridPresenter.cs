/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using MudBlazor;

namespace Blazr.App.Presentation.Mud;

public interface IMudGridPresenter
{
    public IDataResult LastDataResult { get; }
    public int DefaultPageSize { get; set; }
    public List<FilterDefinition>? Filters { get; set; }

    public Task<GridData<TRecord>> GetItemsAsync<TRecord>(GridState<TRecord> request)
            where TRecord : class, new();

}
