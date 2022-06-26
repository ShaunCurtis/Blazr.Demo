/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.UI.Bootstrap;

/// <summary>
/// A display only Input box for formatted text
/// </summary>
public class InputReadOnlyText : ComponentBase
{
    [Parameter] public string Value { get; set; } = String.Empty;

    [Parameter] public bool AsMarkup { get; set; } = true;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(2, "class", "form-control-plaintext form-control-markup");
        if (AsMarkup) builder.AddMarkupContent(4, this.Value);
        else builder.AddContent(4, this.Value);
        builder.CloseElement();
    }
}

public class InputReadOnlyText<TValue> : ComponentBase
{
    [Parameter] public TValue? Value { get; set; }

    [Parameter] public bool AsMarkup { get; set; } = true;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(2, "class", "form-control-plaintext form-control-markup");
        if (AsMarkup) builder.AddMarkupContent(4, this.Value?.ToString());
        else builder.AddContent(4, this.Value?.ToString());
        builder.CloseElement();
    }
}
