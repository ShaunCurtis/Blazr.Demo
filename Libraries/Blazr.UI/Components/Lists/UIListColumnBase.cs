/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI.Bootstrap;

public class UIListColumnBase<TRecord> : UIComponentBase
    where TRecord : class, new()
{
    private bool showSortingDropdown = false;

    [CascadingParameter(Name = "IsHeader")] public bool IsHeader { get; set; }
 
    [CascadingParameter] protected ListContext<TRecord> ListContext { get; set; } = default!;
    
    [Parameter] public bool IsMaxColumn { get; set; }
    
    [Parameter] public string HeaderTitle { get; set; } = string.Empty;

    [Parameter] public bool IsHeaderNoWrap { get; set; } = true;
    
    [Parameter] public bool NoWrap { get; set; }
    
    [Parameter] public string SortField { get; set; } = string.Empty;

    [Parameter] public Expression<Func<TRecord, object>>? SortExpression { get; set; }

    protected string showCss => showSortingDropdown ? "show" : String.Empty;

    protected override Task OnParametersChangedAsync(bool firstRender)
    {
        if (this.ListContext is null)
            throw new NullReferenceException("There's no cascaded ListContext.");
        
        return Task.CompletedTask;
    }

    protected bool isMaxRowColumn 
        => IsMaxColumn && !this.IsHeader;

    protected bool isNormalRowColumn 
        => !IsMaxColumn && !this.IsHeader;

    protected bool _isSortField 
        => !string.IsNullOrWhiteSpace(this.SortField);

    protected void ShowSorting(bool show)
    {
        showSortingDropdown = show;
        Render();
    }

    protected void SortClick(bool descending)
    {
        SortRequest request = this.IsCurrentSortField()
            ? new SortRequest { SortDescending = descending, SortField = this.ListContext.ListState.SortField }
            : new SortRequest { SortDescending = descending, SortField = this.SortField };
         
        this.ListContext?.NotifySortingRequested(this, request);
    }

    protected bool IsCurrentSortField()
    {
        if (string.IsNullOrWhiteSpace(this.ListContext.ListState.SortField))
            return false;

        return this.ListContext.ListState.SortField.Equals(this.SortField);
    }

    protected string GetActive(bool dir)
    {
        bool sortDescending = this.ListContext?.ListState.SortDescending ?? false;

        if (this.IsCurrentSortField())
            return dir == sortDescending ? "active" : string.Empty;

        return string.Empty;
    }
}
