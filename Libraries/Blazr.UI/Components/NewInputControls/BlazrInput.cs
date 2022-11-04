/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.UI;

public class BlazrInput<TValue> : BlazrInputBase<TValue>
{
    [DisallowNull] public ElementReference? Element { get; protected set; }

    protected RenderFragment BaseInputControl;

    public BlazrInput()
        : base()
        => this.BaseInputControl = BuildControl;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
        => this.BuildControl(builder);

    private void BuildControl(RenderTreeBuilder builder)
    {
        var isTextArea = this.Type?.Equals("textarea", StringComparison.InvariantCultureIgnoreCase) ?? false;
        var tag = isTextArea
            ? "textarea"
            : "input";

        builder.OpenElement(0, tag);
        builder.AddMultipleAttributes(1, this.AdditionalAttributes);
        builder.AddAttributeIfTrue(2, !isTextArea, "type", this.Type);
        builder.AddAttributeIfNotNullOrEmpty(3, "class", this.CssClass);
        builder.AddAttribute(4, "value", this.ValueAsString);
        builder.AddAttribute(5, "onchange", this.OnChanged);
        builder.AddElementReferenceCapture(6, __inputReference => this.Element = __inputReference);
        builder.CloseElement();
    }
}
