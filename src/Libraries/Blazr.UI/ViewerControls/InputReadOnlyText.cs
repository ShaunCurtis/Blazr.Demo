/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

/// <summary>
/// A display only Input box for formatted text
/// </summary>
public class InputReadOnlyText : BlazrUIBase
{
    [Parameter] public object Value { get; set; } = default!;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(2, "class", "mx-1");
        builder.AddContent(4, GetAsMarkup(this.Value));
        builder.CloseElement();
    }

    private MarkupString GetAsMarkup(object value)
    { 
        switch (value)
        {
            case MarkupString mValue:
                return mValue;

            case string sValue:
                return (MarkupString)(sValue);

            case null:
                return new MarkupString(string.Empty);

            default:
                return new MarkupString(value?.ToString() ?? String.Empty);
        }
    }
}

public class InputReadOnlyDisplay : BlazrUIBase
{
    [Parameter] public object? Value { get; set; }

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

public class InputReadOnlyDisplay<TValue> : BlazrUIBase
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
