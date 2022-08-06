/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.UI;

public abstract partial class BlazrAppEditorForm<TRecord, TEditRecord, TEntity>
    : BlazrEditorForm<TRecord, TEditRecord, TEntity>
    where TRecord : class, new()
    where TEntity : class, IEntity
    where TEditRecord : class, IEditRecord<TRecord>, new()
{
    protected string ContainerCss => this.Modal is null
        ? "p-2 bg-light border border-brand mt-2"
        : "p-2 bg-light";

    protected virtual RenderFragment? Childcontent => (builder) => this.BuildRenderTree(builder);
    protected abstract RenderFragment BaseContent { get; }

    public BlazrAppEditorForm()
    {
        renderFragment = builder =>
        {
            hasPendingQueuedRender = false;
            hasNeverRendered = false;
            builder.AddContent(0, BaseContent);
        };
    }
}
