/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

/// <summary>
/// Base minimum footprint component for building simple UI Components
/// with single PreRender event method
/// </summary>
public abstract class UIHtmlComponentBase : UIComponentBase
{
    [Parameter] public bool Disabled { get; set; } = false;

    [Parameter] public string? Tag { get; set; }

    [Parameter] public string Class { get; set; } = String.Empty;

    protected virtual string HtmlTag => this.Tag ?? "div";

    protected virtual CSSBuilder CssBuilder => new CSSBuilder().AddClass(this.Class);

    protected string CssClass => this.CssBuilder.Build();

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, this.HtmlTag);
        builder.AddAttributeIfNotEmpty(2, "class", this.CssClass);
        builder.AddAttributeIfTrue(this.Disabled, 3, "disabled");
        builder.AddContentIfNotNull(5, this.ChildContent);
        builder.CloseElement();
    }
}

