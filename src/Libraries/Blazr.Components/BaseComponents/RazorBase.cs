/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Components;

public abstract class RazorBase
{
    protected abstract void BuildRenderTree(RenderTreeBuilder builder);

    public RenderFragment Content => (builder) => BuildRenderTree(builder);
}
