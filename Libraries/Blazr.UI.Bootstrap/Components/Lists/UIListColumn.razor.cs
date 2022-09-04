/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI.Bootstrap;

public partial class UIListColumn : UIComponentBase
{
    private bool showSortingDropdown = false;
    private bool isMaxRowColumn => IsMaxColumn && !this.IsHeader;
    private bool isNormalRowColumn => !IsMaxColumn && !this.IsHeader;
    private bool _isSortField => !string.IsNullOrWhiteSpace(this.SortField);

    [CascadingParameter(Name = "IsHeader")] public bool IsHeader { get; set; }
 
    [CascadingParameter] private ListContext _listContext { get; set; } = default!;
    
    [Parameter] public bool IsMaxColumn { get; set; }
    
    [Parameter] public string HeaderTitle { get; set; } = string.Empty;

    [Parameter] public bool IsHeaderNoWrap { get; set; } = true;
    
    [Parameter] public bool NoWrap { get; set; }
    
    [Parameter] public string SortField { get; set; } = string.Empty;

    private string showCss => showSortingDropdown ? "show" : String.Empty;

    protected override Task OnParametersChangedAsync(bool firstRender)
    {
        if (_listContext is null)
            throw new NullReferenceException("There's no cascaded ListContext.");
        
        return Task.CompletedTask;
    }

    private void ShowSorting(bool show)
    {
        showSortingDropdown = show;
        Render();
    }

    private void SortClick(bool descending)
    {
        SortRequest request = this.IsCurrentSortField()
            ? new SortRequest { SortDescending = descending, SortField = this._listContext.ListState.SortField }
            : new SortRequest { SortDescending = descending, SortField = this.SortField };

        this._listContext?.NotifySortingRequested(this, request);
    }

    private bool IsCurrentSortField()
    {
        if (string.IsNullOrWhiteSpace(_listContext.ListState.SortField))
            return false;

        return _listContext.ListState.SortField.Equals(this.SortField);
    }

    private string GetActive(bool dir)
    {
        bool sortDescending = this._listContext?.ListState.SortDescending ?? false;

        if (this.IsCurrentSortField())
            return dir == sortDescending ? "active" : string.Empty;

        return string.Empty;
    }

    private string HeaderCss
        => CSSBuilder.Class()
            .AddClass(IsHeaderNoWrap, "header-column-nowrap", "header-column")
            .AddClass(NoWrap, "text-nowrap")
            .AddClass("align-baseline")
            .AddClass(this.Class)
            .Build();

    private string TDCss
        => CSSBuilder.Class()
            .AddClass(this.isMaxRowColumn, "max-column", "data-column")
            .AddClass(this.NoWrap, "text-nowrap")
            .AddClass(this.Class)
            .Build();

    private string SortIconCss
    => _listContext is null || !this.IsCurrentSortField()
        ? UICssClasses.NotSortedClass
        : this._listContext.ListState.SortDescending
            ? UICssClasses.AscendingClass
            : UICssClasses.DescendingClass;
}
