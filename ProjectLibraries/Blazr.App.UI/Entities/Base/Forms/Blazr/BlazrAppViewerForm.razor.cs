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

    protected virtual RenderFragment? FormContent => (builder) => this.BuildRenderTree(builder);
    protected abstract RenderFragment BaseContent { get; }

    public BlazrAppViewerForm()
    {
        _renderFragment = builder =>
        {
            _hasPendingQueuedRender = false;
            _hasNeverRendered = false;
            builder.AddContent(0, BaseContent);
        };
    }
    protected virtual AppAuthFields GetAuthFields(TRecord? record)
    => new AppAuthFields { OwnerId = (record as IAuthRecord)?.OwnerId ?? Guid.Empty };

}
