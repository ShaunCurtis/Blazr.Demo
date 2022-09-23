/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

/// <summary>
/// Base minimum footprint component for building simple UI Components
/// with single PreRender event method
/// </summary>
public abstract class UIComponentBase : UIBase
{
    [Parameter] public bool Disabled { get; set; } = false;

    [Parameter] public string? Tag { get; set; }

    protected virtual string HtmlTag => this.Tag ?? "div";

    protected virtual CSSBuilder CssBuilder => new CSSBuilder().AddClass(this.Class);

    protected string CssClass => this.CssBuilder.Build();


    /// <summary>
    /// Method that can be overridden by child components
    /// Eqivalent to OnParametersSetAsync
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns></returns>
    protected virtual Task OnParametersChangedAsync(bool firstRender)
        => Task.CompletedTask;

    /// <summary>
    ///  IComponent implementation
    /// Called by the Renderer at initialization and whenever any of the requested Parameters change
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        var shouldRender = this.ShouldRenderOnParameterChange(initialized);

        if (hasNeverRendered || shouldRender)
        {
            await this.OnParametersChangedAsync(!initialized);
            this.Render();
        }

        this.initialized = true;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, this.HtmlTag);
        builder.AddAttributeIfNotEmpty(2, "class", this.CssClass);
        builder.AddAttributeIfTrue(this.Disabled, 3, "disabled");
        builder.AddContentIfNotNull(5, this.ChildContent);
        builder.CloseElement();
    }

}

