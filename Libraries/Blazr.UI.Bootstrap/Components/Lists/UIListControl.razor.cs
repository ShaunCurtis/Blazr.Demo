/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI.Bootstrap;

public partial class UIListControl<TRecord> : UIHtmlComponentBase
    where TRecord : class, new()
{
    [Parameter] public ComponentState LoadState { get; set; }

    [Parameter] public RenderFragment<TRecord>? RowTemplate { get; set; }

    [Parameter] public RecordList<TRecord>? Records { get; set; }

    [Parameter] public string? HeaderCss { get; set; }

    [CascadingParameter] private ListContext<TRecord>? listContext { get; set; }

    protected override ValueTask<bool> OnParametersChangedAsync(bool firstRender)
    {
        if (listContext is not null)
            listContext.StateChanged += OnStateChanged;

        return ValueTask.FromResult(true);  
    }

    private bool HasRecords
        => Records?.Count() > 0;

    private void OnStateChanged(object? sender, ListState<TRecord> listState)
        => this.StateHasChanged();

    public void Dispose()
    {
        if (listContext is not null)
            listContext.StateChanged -= OnStateChanged;
    }
}
