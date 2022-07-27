/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public record DboWeatherSummary
    : IRecord
{
    [Key]
    public Guid Uid { get; init; } = Guid.Empty;

    public string Summary { get; init; } = string.Empty;
}
