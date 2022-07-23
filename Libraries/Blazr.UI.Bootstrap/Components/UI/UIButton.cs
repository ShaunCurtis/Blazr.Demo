/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.UI.Bootstrap;

public class UIButton : UIComponent
{
    protected override CSSBuilder CssBuilder => base.CssBuilder
        .AddClass("btn mr-1");

    protected override string HtmlTag => "button";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, this.HtmlTag);
        builder.AddAttribute(1, "class", this.CssClass);
        builder.AddMultipleAttributes(2, this.SplatterAttributes);

        if (!UserAttributes.ContainsKey("type"))
            builder.AddAttribute(3, "type", "button");

        if (Disabled)
            builder.AddAttribute(4, "disabled");

        if (ClickEvent.HasDelegate)
            builder.AddAttribute(5, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, ClickEvent));

        builder.AddContent(6, ChildContent);
        builder.CloseElement();
    }
}

