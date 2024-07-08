namespace Blazr.App.UI.Vanilla.Components.Modal;

public class ModalOptions
{
    public string? Title { get; set; }
    public string ModalSize { get; set; } = BsModalSizes.Default;
    public Dictionary<string, object> Parameters { get; set; } =new();
    public Type? Component { get; set; }
}

public static class BsModalSizes
{
    public const string Small = "modal-dialog modal-sm";
    public const string Normal = "modal-dialog";
    public const string Large = "modal-dialog modal-lg";
    public const string ExtraLarge = "modal-dialog modal-xl";
    public const string Default = Large;
}