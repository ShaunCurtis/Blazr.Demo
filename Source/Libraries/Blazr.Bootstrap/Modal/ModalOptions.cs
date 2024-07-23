namespace Blazr.Bootstrap;

public class ModalOptions
{
    public string? Title { get; set; }
    public ModalSize Size { get; set; } = ModalSizes.Default;
    public Dictionary<string, object> Parameters { get; set; } =new();
    public Type? Component { get; set; }
}


public record ModalSize(string Value);

public static class ModalSizes
{
    public static ModalSize Small => new ModalSize("modal-dialog modal-sm");
    public static ModalSize Normal = new ModalSize("modal-dialog");
    public static ModalSize Large = new ModalSize("modal-dialog modal-lg");
    public static ModalSize ExtraLarge = new ModalSize("modal-dialog modal-xl");
    public static ModalSize Default = Large;
}