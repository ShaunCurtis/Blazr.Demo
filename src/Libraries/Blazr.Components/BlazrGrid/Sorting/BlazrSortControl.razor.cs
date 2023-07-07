/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Components.BlazrGrid;

public sealed partial class BlazrSortControl<TGridItem> : BlazrControlBase, IDisposable
     where TGridItem : class, new()
{
    private bool showSortingDropdown = false;

    [CascadingParameter] public IListController<TGridItem>? ListController { get; set; }

    [Parameter] public bool IsMaxColumn { get; set; }

    [Parameter] public string Title { get; set; } = string.Empty;

    [Parameter] public bool IsNoWrap { get; set; } = true;

    [Parameter] public string SortField { get; set; } = string.Empty;

    private string showCss => showSortingDropdown ? "show" : String.Empty;
    private IListController<TGridItem> _listController = default!;

    protected override Task OnParametersSetAsync()
    {
        if (this.ListController is null)
            throw new NullReferenceException("There's no cascaded ListController.");

        _listController = this.ListController;

        if (NotInitialized)
            _listController.StateChanged += this.OnStateChanged;

        return Task.CompletedTask;
    }

    private void ShowSorting(bool show)
    {
        showSortingDropdown = show;
        this.StateHasChanged();
    }

    private async Task SortClick(bool descending)
    {
        var isCurrentSortField = this.IsCurrentSortField();
        SortRequest request = this.IsCurrentSortField()
            ? new SortRequest { SortDescending = descending, SortField = _listController.ListState.SortField }
            : new SortRequest { SortDescending = descending, SortField = this.SortField };

        await this._listController.NotifySortingRequestedAsync(this, new SortEventArgs { Request = request });
    }

    private bool IsCurrentSortField()
    {
        if (string.IsNullOrWhiteSpace(_listController.ListState.SortField))
            return false;

        return _listController.ListState.SortField.Equals(this.SortField);
    }

    private string GetActive(bool dir)
    {
        bool sortDescending = _listController.ListState.SortDescending;

        if (this.IsCurrentSortField())
            return dir == sortDescending ? "active" : string.Empty;

        return string.Empty;
    }

    private string HeaderCss
     => CSSBuilder.Class(BlazrGridCss.HeaderCss)
         .AddClass("align-baseline")
         .Build();

    private string SortIconCss
    => _listController is null || !this.IsCurrentSortField()
        ? BlazrGridCss.NotSortedClass
        : _listController.ListState.SortDescending
            ? BlazrGridCss.AscendingClass
            : BlazrGridCss.DescendingClass;

    private void OnStateChanged(object? sender, EventArgs e)
        => this.StateHasChanged();

    public void Dispose()
        => _listController.StateChanged -= this.OnStateChanged;
}
