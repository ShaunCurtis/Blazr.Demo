﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI.Bootstrap;

public partial class UICard : CoreComponentBase
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

    protected bool Collapsed { get; set; } = false;

    protected string CollapseText { get => this.Collapsed ? "Show" : "Hide"; }

    protected string CardCss => new CSSBuilder()
        .AddClass(Dirty, "card border-danger", "card border-dark")
        .AddClass(this.CardClass)
        .Build();

    protected string CardHeaderCss => new CSSBuilder()
        .AddClass(Dirty, "card-header text-white bg-danger", "card-header text-brand bg-dark")
        .AddClass(this.HeaderClass)
        .Build();

    protected string CardHeaderButtonCss => new CSSBuilder()
        .AddClass("btn")
        .AddClass(this.CardButtonCSS)
        .AddClass("float-right p-1")
        .Build();

    protected string CardBodyCss => new CSSBuilder()
        .AddClass("card-body card-body-light p-0")
        .AddClass(this.Collapsed, "collapse", "collapse show")
        .AddClass(this.BodyClass)
        .Build();

    protected void Toggle()
        => this.Collapsed = !this.Collapsed;
}
