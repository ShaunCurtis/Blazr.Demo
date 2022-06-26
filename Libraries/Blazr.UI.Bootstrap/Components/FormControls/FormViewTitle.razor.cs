/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.UI.Bootstrap;

public partial class FormViewTitle
{
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter] public string Title { get; set; } = "No Title Set";
}

