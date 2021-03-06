/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public class BlazrInputTextArea : InputTextArea, IComponentReference
{
    [Parameter] public bool BindOnInput { get; set; } = true;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "textarea");
        builder.AddMultipleAttributes(1, AdditionalAttributes);

        if (!string.IsNullOrWhiteSpace(this.CssClass))
            builder.AddAttribute(2, "class", CssClass);

        builder.AddAttribute(3, "value", BindConverter.FormatValue(CurrentValueAsString));

        if (BindOnInput)
            builder.AddAttribute(4, "oninput", EventCallback.Factory.CreateBinder<string?>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
        else
            builder.AddAttribute(5, "onchange", EventCallback.Factory.CreateBinder<string?>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));

        builder.AddElementReferenceCapture(6, __inputReference => Element = __inputReference);
        builder.CloseElement();
    }
}
