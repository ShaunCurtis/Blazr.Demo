using Microsoft.AspNetCore.Components.Web.Virtualization;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.UI.Bootstrap;

public partial class UIVirtualizeListControl<TRecord> : UIComponentBase
{
    [Parameter] [EditorRequired] public RenderFragment<TRecord>? RowTemplate { get; set; }

    [Parameter] public int Columns { get; set; } = 3;

    [Parameter] [EditorRequired] public Func<ItemsProviderRequest, ValueTask<ItemsProviderResult<TRecord>>>? ItemsProvider { get; set; }

    private Virtualize<TRecord>? VirtualizeComponent;

    private async ValueTask<ItemsProviderResult<TRecord>> GetItems(ItemsProviderRequest request)
        => ItemsProvider is not null
            ? await ItemsProvider(request)
            : new ItemsProviderResult<TRecord>(new List<TRecord>(), 0);

    public async void NotifyListChanged()
    {
        if (this.VirtualizeComponent is not null)
            await this.InvokeAsync(async () =>
            {
                await VirtualizeComponent.RefreshDataAsync();
                StateHasChanged();
            });
    }
}

