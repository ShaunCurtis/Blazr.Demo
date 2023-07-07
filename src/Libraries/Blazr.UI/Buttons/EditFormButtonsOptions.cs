/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public record EditFormButtonsOptions
{
    public string SaveText { get; init; } = "Save";
    public string UpdateText { get; init; } = "Update";
    public string ResetText { get; init; } = "Reset";
    public string ExitText { get; init; } = "Exit";
    public string ExitWithoutSaveText { get; init; } = "Exit without Saving";
}
