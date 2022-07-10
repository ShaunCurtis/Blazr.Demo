/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public class BlazrInputNumber<TValue> : InputNumber<TValue>, IComponentReference
{
    [Parameter] public bool BindOnInput { get; set; } = true;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "input");
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddAttribute(2, "type", "number");

        if (!string.IsNullOrWhiteSpace(this.CssClass))
            builder.AddAttribute(3, "class", CssClass);

        builder.AddAttribute(4, "value", BindConverter.FormatValue(CurrentValueAsString));

        if (BindOnInput)
            builder.AddAttribute(5, "oninput", EventCallback.Factory.CreateBinder<string?>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
        else
            builder.AddAttribute(6, "onchange", EventCallback.Factory.CreateBinder<string?>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));

        builder.AddElementReferenceCapture(7, __inputReference => Element = __inputReference);
        builder.CloseElement();
    }
}
