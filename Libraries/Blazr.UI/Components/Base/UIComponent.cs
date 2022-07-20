/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.UI;

public class UIComponent : UIComponentBase
{

    [Parameter] public bool Show { get; set; } = true;

    [Parameter] public bool Disabled { get; set; } = false;

    [Parameter] public string? Tag { get; set; }

    [Parameter] public EventCallback<MouseEventArgs> ClickEvent { get; set; }

    protected virtual List<string> CssClasses { get; private set; } = new List<string>();

    protected virtual string HtmlTag => this.Tag ?? "div";

    protected override List<string> UnwantedAttributes { get; set; } = new List<string>() { "class" };

    protected string CssClass
        => CSSBuilder.Class()
            .AddClass(CssClasses)
            .AddClassFromAttributes(this.UserAttributes)
            .Build();

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (this.Show)
        {
            builder.OpenElement(0, this.HtmlTag);
            builder.AddMultipleAttributes(1, this.SplatterAttributes);
            if (!string.IsNullOrWhiteSpace(this.CssClass))
                builder.AddAttribute(2, "class", this.CssClass);

            if (Disabled)
                builder.AddAttribute(3, "disabled");

            if (ClickEvent.HasDelegate)
                builder.AddAttribute(4, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, ClickEvent));

            if (this.ChildContent is not null)
                builder.AddContent(5, this.ChildContent);

            builder.CloseElement();
        }
    }
}

