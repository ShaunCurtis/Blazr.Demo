/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.UI;

public abstract partial class BlazrAppPagedListForm<TRecord, TEntity>
    : BlazrPagedListForm<TRecord, TEntity>
    where TRecord : class, new()
    where TEntity : class, IEntity
{
    protected readonly string TableCss = "table table-sm table-striped table-hover border-bottom no-margin";
    protected string ContainerCss = "bg-light border border-brand p-0 mt-2";

    protected RenderFragment? Childcontent => (builder) => this.BuildRenderTree(builder);
    protected RenderFragment? TemplateContent { get; set; }

    public BlazrAppPagedListForm()
    {
        componentRenderFragment = builder =>
        {
            hasPendingQueuedRender = false;
            hasNeverRendered = false;
            TemplateContent?.Invoke(builder);
        };
    }

    protected virtual AppAuthFields GetAuthFields(TRecord record)
        =>  new AppAuthFields { OwnerId = (record as IAuthRecord)?.OwnerId ?? Guid.Empty};
}
