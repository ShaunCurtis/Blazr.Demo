/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

/// <summary>
/// Base minimum footprint component for building UI Components
/// with single PreRender event method
/// </summary>
public abstract class UIComponentBase : UICoreComponentBase
{
    protected bool initialized;

    /// <summary>
    /// Method that can be overridden by child components
    /// Eqivalent to OnParametersSetAsync
    /// Return false to prevent a Render.
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns>True to call StateHasChanged</returns>
    protected virtual ValueTask<bool> OnParametersChangedAsync(bool firstRender)
        => ValueTask.FromResult(true);

    /// <summary>
    ///  IComponent implementation
    /// Called by the Renderer at initialization and whenever any of the requested Parameters change
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        var dorender = await this.OnParametersChangedAsync(!initialized)
            || hasNeverRendered
            || !hasPendingQueuedRender;

            if (dorender)
                this.StateHasChanged();

        this.initialized = true;
    }
}
