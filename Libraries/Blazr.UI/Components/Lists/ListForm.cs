/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public class ListForm : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter] [EditorRequired] public ListContext? ListContext { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<CascadingValue<ListContext>>(0);
        builder.AddAttribute(1, "value", this.ListContext);
        builder.AddAttribute(2, "ChildContent", ChildContent);
        builder.CloseComponent();
    }
}
