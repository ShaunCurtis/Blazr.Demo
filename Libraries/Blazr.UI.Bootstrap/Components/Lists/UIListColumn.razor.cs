/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI.Bootstrap;

public partial class UIListColumn : UIBase
{
    private bool isMaxRowColumn => IsMaxColumn && !this.IsHeader;
    private bool isNormalRowColumn => !IsMaxColumn && !this.IsHeader;
    private bool _isSortField => !string.IsNullOrWhiteSpace(this.SortField);
    protected override List<string> UnwantedAttributes { get; set; } = new List<string>() { "class" };

    [CascadingParameter(Name = "IsHeader")] public bool IsHeader { get; set; }
 
    [CascadingParameter] private ListContext? _listContext { get; set; }
    
    [Parameter] public bool IsMaxColumn { get; set; }
    
    [Parameter] public string HeaderTitle { get; set; } = string.Empty;

    [Parameter] public bool IsHeaderNoWrap { get; set; } = true;
    
    [Parameter] public bool NoWrap { get; set; }
    
    [Parameter] public string SortField { get; set; } = string.Empty;

    private void SortClick(MouseEventArgs e)
    {
        if (this._listContext is null)
            return;

        // TODO - currwnt sort issue is here
        SortRequest request = this.IsCurrentSortField()
            ?  new SortRequest { SortDescending = !this._listContext.SortDescending, SortField = this._listContext.SortField }
            : new SortRequest { SortDescending = false, SortField = this.SortField };

        this._listContext?.SortAsync(request);
    }
    private bool IsCurrentSortField()
    {
        if (this._listContext is null || String.IsNullOrWhiteSpace(_listContext.SortField))
            return false;

        return _listContext.SortField.Equals(this.SortField);
    }


    private string HeaderCss
        => CSSBuilder.Class()
            .AddClass(IsHeaderNoWrap, "header-column-nowrap", "header-column")
            .AddClass(NoWrap, "text-nowrap")
            .AddClass("align-baseline")
            .AddClassFromAttributes(UserAttributes)
            .Build();

    private string TDCss
        => CSSBuilder.Class()
            .AddClass(this.isMaxRowColumn, "max-column", "data-column")
            .AddClass(this.NoWrap, "text-nowrap")
            .AddClassFromAttributes(UserAttributes)
            .Build();

    private string SortIconCss
    => _listContext is null || !this.IsCurrentSortField()
        ? UICssClasses.NotSortedClass
        : this._listContext.SortDescending
            ? UICssClasses.AscendingClass
            : UICssClasses.DescendingClass;
}

