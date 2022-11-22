/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public class BlazrSelect<TValue> : BlazrInputBase<TValue>
{
    private readonly bool _isMultipleSelect;
    protected RenderFragment BaseInputControl;

    public BlazrSelect()
        : base()
    {
        _isMultipleSelect = typeof(TValue).IsArray;
        this.BaseInputControl = BuildControl;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
        => this.BuildControl(builder);

    private void BuildControl(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "select");
        builder.AddMultipleAttributes(1, this.AdditionalAttributes);
        builder.AddAttributeIfNotEmpty(2, "class", this.CssClass);
        builder.AddAttributeIfTrue(3, _isMultipleSelect,  "multiple", _isMultipleSelect);
        builder.AddAttribute(4, "value", this.ValueAsString);
        builder.AddAttribute(5, "onchange", this.OnChanged);
        builder.AddElementReferenceCapture(6, __inputReference => this.Element = __inputReference);
        builder.AddContent(7, this.ChildContent);
        builder.CloseElement();
    }
}
