/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.UI.Bootstrap;

public enum BootstrapSize { ExtraSmall, Small, Medium, Large, XLarge, XXLarge, Fluid }

public class UIContainer : UIBlock
{
    [Parameter] public BootstrapSize Size { get; set; } = BootstrapSize.Fluid;

    private string _css => Size switch
    {
        BootstrapSize.Small => "container-sm",
        BootstrapSize.Medium => "container-md",
        BootstrapSize.Large => "container-lg",
        BootstrapSize.XLarge => "container-xl",
        BootstrapSize.XXLarge => "container-xxl",
        _ => "container-fluid"
    };

    protected override CSSBuilder CssBuilder => base.CssBuilder.AddClass(_css);
}

