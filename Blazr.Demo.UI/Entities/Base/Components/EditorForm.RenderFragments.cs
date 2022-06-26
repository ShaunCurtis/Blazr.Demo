/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.UI;

public abstract partial class EditorForm<TRecord, TEditRecord, TService>
    : OwningComponentBase, IDisposable
    where TRecord : class, new()
    where TEditRecord : class, IEditRecord<TRecord>, new()
    where TService : class, IEntityService
{

    protected void ButtonBuilder(RenderTreeBuilder builder, string cssclass, bool? show, bool? disable, Action back, string text)
    {
        builder.OpenComponent<UIButton>(0);
        builder.AddAttribute(1, "class", cssclass);
        if (show is not null)
            builder.AddAttribute(2, "Show", show);

        if (disable is not null)
            builder.AddAttribute(2, "Disable", disable);
 
        builder.AddAttribute(3, "ClickEvent", EventCallback.Factory.Create<MouseEventArgs>(this, back));
        RenderFragment content = (_builder) =>
        {
            _builder.AddMarkupContent(0, text);
        };

        builder.AddAttribute(4, "ChildContent", content);
        builder.CloseComponent();
    }
}
