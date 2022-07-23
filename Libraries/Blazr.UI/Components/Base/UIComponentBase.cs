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
    protected bool _initialized = false;

    /// <summary>
    /// Method that can be overridden by child components
    /// Eqivalent to OnParametersSetAsync
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns></returns>
    protected virtual Task OnPreRenderAsync(bool firstRender)
        => Task.CompletedTask;

    /// <summary>
    /// Classic StateHasChangedMethod
    /// Do not call through InvokeAsync, it already does it.
    /// </summary>
    public void StateHasChanged()
        => this.InvokeAsync(this.Render);

    /// <summary>
    ///  IComponent implementation
    /// Called by the Renderer at initialization and whenever any of the requested Parameters change
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        await this.OnPreRenderAsync(!_initialized);
        this.Render();
        _initialized = true;
    }
}

