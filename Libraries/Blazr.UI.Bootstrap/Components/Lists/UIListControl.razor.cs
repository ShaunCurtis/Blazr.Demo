﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI.Bootstrap;

public partial class UIListControl<TRecord> : UIComponentBase
{
    [Parameter] public ComponentState LoadState { get; set; }

    [Parameter] public RenderFragment<TRecord>? RowTemplate { get; set; }

    [Parameter] public IEnumerable<TRecord>? Records { get; set; }

    [Parameter] public string? HeaderCss { get; set; }

    [CascadingParameter] private ListContext? listContext { get; set; }

    private bool HasRecords => Records?.Count() > 0;

    protected override Task OnParametersChangedAsync(bool firstRender)
    {
        if (listContext is not null)
            listContext.StateUpdated += OnStateUpdated;

        return base.OnParametersChangedAsync(firstRender);
    }

    private void OnStateUpdated(object? sender, ListState listState)
        => this.StateHasChanged();

    public void Dispose()
    {
        if (listContext is not null)
            listContext.StateUpdated -= OnStateUpdated;
    }

}

