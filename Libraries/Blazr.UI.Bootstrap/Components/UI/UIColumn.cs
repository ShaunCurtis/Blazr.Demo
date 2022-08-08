/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.UI.Bootstrap;

public class UIColumn : UIComponent
{
    [Parameter] public int Columns { get; set; } = 0;

    [Parameter] public int SmallColumns { get; set; } = 0;

    [Parameter] public int MediumColumns { get; set; } = 0;

    [Parameter] public int LargeColumns { get; set; } = 0;

    [Parameter] public int XLargeColumns { get; set; } = 0;

    [Parameter] public int XXLargeColumns { get; set; } = 0;

    [Parameter] public bool AutoDefault { get; set; } = false;

    protected override CSSBuilder CssBuilder => base.CssBuilder
        .AddClass(AutoDefault, "col-auto", "col")
        .AddClass(Columns > 0 && !AutoDefault, $"col-{this.Columns}", $"col-12")
        .AddClass(SmallColumns > 0, $"col-sm-{this.SmallColumns}")
        .AddClass(MediumColumns > 0, $"col-md-{this.MediumColumns}")
        .AddClass(LargeColumns > 0, $"col-lg-{this.LargeColumns}")
        .AddClass(XLargeColumns > 0, $"col-xl-{this.XLargeColumns}")
        .AddClass(XXLargeColumns > 0, $"col-xxl-{this.XXLargeColumns}");
}

