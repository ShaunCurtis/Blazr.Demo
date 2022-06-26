/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.UI.Bootstrap;

/// <summary>
/// A display only Input box for Bools
/// </summary>
public class InputReadOnlyActive : ComponentBase
{
    [Parameter] public bool Value { get; set; } = false;

    [Parameter] public bool AsMarkup { get; set; } = true;

    [Parameter] public string IsTrueValue { get; set; } = "On";

    [Parameter] public string IsFalseValue { get; set; } = "Off";

    private string css => new CSSBuilder("btn btn-sm")
        .AddClass(this.Value, "btn-success", "btn-outline-danger")
        .Build();

    private string message => this.Value ? "Active" : "Hidden";
        

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.OpenElement(1, "button");
        builder.AddAttribute(2, "class", this.css);
        builder.AddAttribute(3, "disabled", true);
        builder.AddMarkupContent(4, this.message);
        builder.CloseElement();
        builder.CloseElement();
    }
}
