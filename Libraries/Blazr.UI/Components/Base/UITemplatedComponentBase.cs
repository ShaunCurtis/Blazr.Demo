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
public abstract class UITemplatedComponentBase : UIComponentBase
{
    protected virtual void BuildComponent(RenderTreeBuilder builder)
    {
        if (this.TemplatedContent is not null)
        {
            this.TemplatedContent.Invoke(builder);
            return;
        }

        BuildRenderTree(builder);
    }

    protected virtual RenderFragment? TemplatedContent { get; }

    protected RenderFragment? Content => (builder) => this.BuildRenderTree(builder);

    public UITemplatedComponentBase()
    {
        this.renderFragment = builder =>
        {
            hasPendingQueuedRender = false;
            hasNeverRendered = false;
            if (!(this.hide | this.Hidden))
                this.BuildComponent(builder);
        };
    }
}
