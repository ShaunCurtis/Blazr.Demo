/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.UI;

public abstract partial class BlazrAppViewerForm<TRecord, TEntity>
    : BlazrViewerForm<TRecord, TEntity>
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    protected string ContainerCss => this.Modal is null
        ? "p-2 bg-light border border-brand mt-2"
        : "p-2 bg-light";

    protected RenderFragment? FormContent => (builder) => this.BuildRenderTree(builder);
    protected RenderFragment? TemplateContent { get; set; }

    public BlazrAppViewerForm()
    {
        componentRenderFragment = builder =>
        {
            hasPendingQueuedRender = false;
            hasNeverRendered = false;
            TemplateContent?.Invoke(builder);
        };
    }

    protected virtual AppAuthFields GetAuthFields(TRecord? record)
    => new AppAuthFields { OwnerId = (record as IAuthRecord)?.OwnerId ?? Guid.Empty };
}
