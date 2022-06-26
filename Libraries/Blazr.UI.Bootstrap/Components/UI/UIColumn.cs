/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.UI.Bootstrap;

public class UIColumn : UIComponent
{
    [Parameter] public virtual int Columns { get; set; } = 0;

    [Parameter] public virtual int SmallColumns { get; set; } = 0;

    [Parameter] public virtual int MediumColumns { get; set; } = 0;

    [Parameter] public virtual int LargeColumns { get; set; } = 0;

    [Parameter] public virtual int XLargeColumns { get; set; } = 0;

    [Parameter] public virtual int XXLargeColumns { get; set; } = 0;

    protected override void OnInitialized()
    {
        CssClasses.Add($"col");
        if (Columns > 0)
            CssClasses.Add($"col-{this.Columns}");
        if (SmallColumns > 0)
            CssClasses.Add($"col-sm-{this.SmallColumns}");
        if (MediumColumns > 0)
            CssClasses.Add($"col-md-{this.MediumColumns}");
        if (LargeColumns > 0)
            CssClasses.Add($"col-lg-{this.LargeColumns}");
        if (XLargeColumns > 0)
            CssClasses.Add($"col-xl-{this.XLargeColumns}");
        if (XXLargeColumns > 0)
            CssClasses.Add($"col-xxl-{this.XXLargeColumns}");
    }
}

