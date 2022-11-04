/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.UI;

public class BlazrFormMessage
{
    public string? alertMessage { get; set; }
    public string alertColour { get; set; } = "alert-info";
    public int alertTimeOut { get; set; } = 0;
    protected Guid alertId { get; set; } = Guid.NewGuid();

    public bool SetMessage(string message, string colour)
    {
        this.alertMessage = message;
        this.alertColour = colour;
        this.alertTimeOut = 0;
        this.alertId = Guid.NewGuid();
        return true;
    }

    public bool SetMessage(string message, AlertType type)
    {
        this.alertMessage = message;
        this.alertColour = GetColour(type);
        this.alertTimeOut = 0;
        this.alertId = Guid.NewGuid();
        return true;
    }

    public static string GetColour(AlertType value) =>
        value switch
        {
            AlertType.Info => "alert-info",
            AlertType.Warning => "alert-warning",
            AlertType.Failure => "alert-danger",
            AlertType.Success => "alert-success",
            _ => "alert-primary"
        };

}
