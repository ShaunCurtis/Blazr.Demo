/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.UI.Bootstrap;

public class UIButtonGroup : UIComponent
{
    public UIButtonGroup()
        => this.CssClasses.Add("btn-group me-1");

    protected override string HtmlTag => "div";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (this.Show)
        {
            builder.OpenElement(0, this.HtmlTag);
            builder.AddAttribute(1, "class", this.CssClass);
            builder.AddAttribute(2, "role", "group");
            builder.AddMultipleAttributes(3, this.SplatterAttributes);
            builder.AddContent(4, ChildContent);
            builder.CloseElement();
        }
    }
}

