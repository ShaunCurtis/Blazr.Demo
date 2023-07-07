/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Components.BlazrGrid;

public interface IBlazrGridColumn<TGridItem>
{
    public Guid ComponentUid { get; }

    public RenderFragment? ItemHeaderContent { get; }

    public RenderFragment<TGridItem> ItemRowContent { get; }   
}
