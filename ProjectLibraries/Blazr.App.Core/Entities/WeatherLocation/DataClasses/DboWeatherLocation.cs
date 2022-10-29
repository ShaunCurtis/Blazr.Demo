/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.ComponentModel.DataAnnotations;

namespace Blazr.App.Core;

public record DboWeatherLocation
    : IAuthRecord, IRecord
{
    [Key]
    public Guid Uid { get; init; }

    public Guid OwnerId { get; init; }

    public string Location { get; init; } = string.Empty;
}
