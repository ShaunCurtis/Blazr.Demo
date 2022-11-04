/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI.Bootstrap;

public sealed partial class UICard : UIBase
{
    [Parameter] public RenderFragment? Header { get; set; }

    [Parameter] public bool Dirty { get; set; } = false;

    [Parameter] public string? CardClass { get; set; }

    [Parameter] public string? HeaderClass { get; set; }

    [Parameter] public string? BodyClass { get; set; }

    [Parameter] public RenderFragment? Body { get; set; }

    [Parameter] public RenderFragment? Navigation { get; set; }

    [Parameter] public RenderFragment? Buttons { get; set; }

    [Parameter] public string CardButtonCSS { get; set; } = "btn-sm btn-outline-primary";

    [Parameter] public bool IsCollapsible { get; set; } = false;

    private bool Collapsed { get; set; } = false;

    private string CollapseText { get => this.Collapsed ? "Show" : "Hide"; }

    private string CardCss => new CSSBuilder()
        .AddClass(Dirty, "card border-danger", "card border-dark")
        .AddClass(this.CardClass)
        .Build();

    private string CardHeaderCss => new CSSBuilder()
        .AddClass(Dirty, "card-header text-white bg-danger", "card-header text-brand bg-dark")
        .AddClass(this.HeaderClass)
        .Build();

    private string CardHeaderButtonCss => new CSSBuilder()
        .AddClass("btn")
        .AddClass(this.CardButtonCSS)
        .AddClass("float-right p-1")
        .Build();

    private string CardBodyCss => new CSSBuilder()
        .AddClass("card-body card-body-light p-0")
        .AddClass(this.Collapsed, "collapse", "collapse show")
        .AddClass(this.BodyClass)
        .Build();

    private void Toggle()
        => this.Collapsed = !this.Collapsed;
}
