/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Components.BlazrGrid;

public abstract class BlazrGridColumnBase<TGridItem> : BlazrBaseComponent, IComponent, IBlazrGridColumn<TGridItem>
{

    [Parameter] public RenderFragment<TGridItem>? ChildContent { get; set; }

    [Parameter, EditorRequired] public string Title { get; set; } = "Field";

    [Parameter] public bool IsMaxColumn { get; set; } = false;

    [Parameter] public bool IsNoWrap { get; set; } = false;

    [Parameter] public string? Class { get; set; }

    [CascadingParameter] private Action<IBlazrGridColumn<TGridItem>>? Register { get; set; }

    public virtual Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (Register != null)
            Register(this);

        return Task.CompletedTask;
    }

    public virtual RenderFragment? ItemHeaderContent => (builder) =>
    {
        var css = new CSSBuilder(BlazrGridCss.HeaderCss)
            .AddClass("align-baseline")
            .Build();

        builder.OpenElement(0, "th");
        builder.AddAttribute(1, "class", css);
        builder.AddMarkupContent(2, this.Title);
        builder.CloseElement();
    };

    public virtual RenderFragment<TGridItem> ItemRowContent => (item) => (builder) =>
    {
        var css = new CSSBuilder(BlazrGridCss.ItemRowCss)
            .AddClass(this.IsMaxColumn, BlazrGridCss.MaxColumnCss)
            .AddClass(!this.IsMaxColumn && IsNoWrap, BlazrGridCss.NoWrapCss)
            .AddClass(this.Class)
            .Build();

        builder.OpenElement(0, "td");
        builder.AddAttribute(1, "class", css);
        builder.AddAttribute(2, "key", item);
        if (this.ChildContent is not null)
            builder.AddContent(10, ChildContent(item));

        builder.CloseComponent();
    };
}
