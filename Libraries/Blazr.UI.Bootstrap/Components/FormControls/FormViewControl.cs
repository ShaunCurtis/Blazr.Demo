/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.UI.Bootstrap;

public class FormViewControl<TValue> : ComponentBase
{
    [Parameter]
    public TValue? Value { get; set; }

    [Parameter] public string? Label { get; set; }

    [Parameter] public string DivCssClass { get; set; } = "mb-2";

    [Parameter] public string LabelCssClass { get; set; } = "form-label small text-muted";

    [Parameter] public Type ControlType { get; set; } = typeof(InputReadOnlyText<TValue>);

    [Parameter] public bool ShowLabel { get; set; } = true;

    [Parameter] public bool IsRow { get; set; }

    private readonly string formId = Guid.NewGuid().ToString();

    private bool IsLabel => !string.IsNullOrEmpty(this.Label);

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (IsRow)
            builder.AddContent(1, RowFragment);
        else
            builder.AddContent(2, BaseFragment);
    }

    private RenderFragment BaseFragment => (builder) =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(10, "class", this.DivCssClass);
        builder.AddContent(40, this.LabelFragment);
        builder.AddContent(60, this.ControlFragment);
        builder.CloseElement();
    };

    private RenderFragment RowFragment => (builder) =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(10, "class", "row form-group");
        builder.OpenElement(20, "div");
        builder.AddAttribute(30, "class", "col-12 col-md-3");
        builder.AddContent(40, this.LabelFragment);
        builder.CloseElement();
        builder.OpenElement(40, "div");
        builder.AddAttribute(50, "class", "col-12 col-md-9");
        builder.AddContent(60, this.ControlFragment);
        builder.CloseElement();
        builder.CloseElement();
    };

    private RenderFragment LabelFragment => (builder) =>
    {
        if (this.IsLabel)
        {
            builder.OpenElement(110, "label");
            builder.AddAttribute(130, "class", this.LabelCssClass);
            builder.AddMarkupContent(140, this.Label);
            builder.CloseElement();
        }
    };

    private RenderFragment ControlFragment => (builder) =>
    {
        builder.OpenComponent(210, this.ControlType);
        builder.AddAttribute(230, "Value", this.Value);
        builder.CloseComponent();
    };
}

