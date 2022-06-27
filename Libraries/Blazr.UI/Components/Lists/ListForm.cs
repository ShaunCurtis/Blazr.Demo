/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public class ListForm : ComponentBase
{
    [Parameter] public Guid StateId { get; set; } = Guid.Empty;

    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter] [EditorRequired] public ListContext? ListContext { get; set; }

    [Parameter] [EditorRequired] public Func<ListState, ValueTask<ListState>>? ListProvider { get; set; }

    [Inject] private UiStateService? _uiStateService { get; set; }

    private UiStateService UiStateService => _uiStateService!;

    protected override void OnInitialized()
        => ListContext?.Attach(UiStateService, this.StateId, CallListProvider);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<CascadingValue<ListContext>>(0);
        builder.AddAttribute(1, "value", this.ListContext);
        builder.AddAttribute(2, "ChildContent", ChildContent);
        builder.CloseComponent();
    }

    internal async ValueTask<ListState> CallListProvider(ListState options)
    {
        ListState returnOptions = new();
        if (ListProvider is not null)
           returnOptions = await ListProvider(options);
        return returnOptions;
    }
}
