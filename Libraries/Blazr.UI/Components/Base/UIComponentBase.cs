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

        if (hasNeverRendered || shouldRender || renderHandle.IsRenderingOnMetadataUpdate)
        {
            await this.OnParametersChangedAsync(!initialized);
            this.Render();
        }

        this.initialized = true;
    }

}

