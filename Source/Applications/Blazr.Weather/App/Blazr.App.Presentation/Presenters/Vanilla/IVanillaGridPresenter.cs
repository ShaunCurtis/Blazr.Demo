/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.App.Presentation.Vanilla;

public interface IVanillaGridPresenter<TRecord>
    where TRecord : class, new()
{
    public IDataResult LastDataResult { get; }
    public int DefaultPageSize { get; set; }
    public List<FilterDefinition>? Filters { get; set; }

    public ValueTask<GridItemsProviderResult<TRecord>> GetItemsAsync<TGridItem>(GridItemsProviderRequest<TRecord> request);
}
