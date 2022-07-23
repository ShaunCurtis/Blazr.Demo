/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.UI.Bootstrap;

public class UIButton : UIComponent
{
    [Parameter] public UIButtonType ButtonType { get; set; } = UIButtonType.None;

    protected override CSSBuilder CssBuilder => base.CssBuilder
        .AddClass("btn")
        .AddClass("btn-sm")
        .AddClass(this.ButtonCssColour);

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

    protected string ButtonCssColour =>
        ButtonType switch
        {
            UIButtonType.View => "btn-secondary",
            UIButtonType.Dash => "btn-primary",
            UIButtonType.New => "btn-success",
            UIButtonType.Edit => "btn-danger",
            UIButtonType.Exit => "btn-dark",
            UIButtonType.ExitWithoutSave => "btn-danger",
            UIButtonType.Warning => "btn-warning",
            UIButtonType.Save => "btn-success",
            _ => "btn-secondary",
        };
}

public enum UIButtonType
{
    None,
    View,
    Dash,
    New,
    Edit,
    Exit,
    ExitWithoutSave,
    Warning,
    Save
}

