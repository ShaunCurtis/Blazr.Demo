/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.UI;

public class UIButton : HtmlElementBase
{
    [Parameter] public UIButtonColourType ButtonColourType { get; set; } = UIButtonColourType.None;

    [Parameter] public UIButtonSize ButtonSize { get; set; } = UIButtonSize.Small;

    [Parameter] public string Type { get; set; } = "button";

    [Parameter] public EventCallback<MouseEventArgs> ClickEvent { get; set; }

    protected override CSSBuilder CssBuilder => base.CssBuilder
        .AddClass("btn")
        .AddClass(this.ButtonCssSize)
        .AddClass(this.ButtonCssColour);

    protected override string HtmlTag => "button";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, this.HtmlTag);
        builder.AddAttribute(1, "class", this.CssClass);
        builder.AddAttribute(2, "type", this.Type);
        builder.AddAttributeIfTrue(3, this.Disabled, "disabled");
        builder.AddAttributeIfTrue(4, this.Hidden, "hidden", true);
        builder.AddAttributeIfTrue(5, ClickEvent.HasDelegate, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, ClickEvent));
        builder.AddContent(6, ChildContent);
        builder.CloseElement();
    }

    protected string ButtonCssColour =>
        ButtonColourType switch
        {
            UIButtonColourType.View => "btn-secondary",
            UIButtonColourType.Dash => "btn-primary",
            UIButtonColourType.New => "btn-outline-info",
            UIButtonColourType.Edit => "btn-primary",
            UIButtonColourType.Delete => "btn-dark",
            UIButtonColourType.Exit => "btn-outline-primary",
            UIButtonColourType.ExitWithoutSave => "btn-danger",
            UIButtonColourType.Warning => "btn-warning",
            UIButtonColourType.Save => "btn-success",
            UIButtonColourType.Reset => "btn-outline-warning",
            _ => "btn-secondary",
        };

    protected string ButtonCssSize =>
        ButtonSize switch
        {
            UIButtonSize.Small => "btn-sm",
            UIButtonSize.Large => "btn-lg",
            _ => string.Empty,
        };
}

public enum UIButtonColourType
{
    None,
    View,
    Dash,
    New,
    Edit,
    Exit,
    ExitWithoutSave,
    Warning,
    Save,
    Delete,
    Reset
}

public enum UIButtonSize
{
    None,
    Small,
    Normal,
    Large
}
