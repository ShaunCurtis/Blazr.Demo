/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Core;

public class AddWeatherForecastCommand
    : IHandlerRequest<ValueTask<CommandResult>>
{
    public Guid TransactionId { get; } = Guid.NewGuid();

    public DboWeatherForecast Record { get; private set; } = default!;

    public AddWeatherForecastCommand(DboWeatherForecast record)
        => this.Record = record;
}
