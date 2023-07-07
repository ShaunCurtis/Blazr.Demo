/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public class BSModalOptions : ModalOptions
{
    public string ModalSize { get; set; } = BsModalSizes.Default;
}

public static class BsModalSizes
{
    public const string Small = "modal-dialog modal-sm";
    public const string Normal = "modal-dialog";
    public const string Large = "modal-dialog modal-lg";
    public const string ExtraLarge = "modal-dialog modal-xl";
    public const string Default = Large;

}

