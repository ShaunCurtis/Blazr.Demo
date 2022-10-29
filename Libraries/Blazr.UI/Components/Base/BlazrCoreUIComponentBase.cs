/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.UI;

/// <summary>
/// Base footprint component for building simple UI Components
/// No events
/// </summary>
public abstract class BlazrCoreUIComponentBase : BlazrComponentBase
{
    protected bool hide = false;

    /// <summary>
    /// Content to render within the component
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Parameter to control the display of the component
    /// </summary>
    [Parameter] public bool Hidden { get; set; }

    /// <summary>
    /// New method
    /// caches a copy of the Render code
    /// Detects if the component shoud be rendered and if not doesn't render ant content
    /// </summary>
    public BlazrCoreUIComponentBase()
    {
        this.renderFragment = builder =>
        {
            hasPendingQueuedRender = false;
            hasNeverRendered = false;
            if (!(this.hide | this.Hidden))
                this.BuildRenderTree(builder);
        };
    }
}
