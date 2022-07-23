/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI.Bootstrap;

class UIRow : UIBlock
{
    protected override CSSBuilder CssBuilder => base.CssBuilder.AddClass("row");
}

