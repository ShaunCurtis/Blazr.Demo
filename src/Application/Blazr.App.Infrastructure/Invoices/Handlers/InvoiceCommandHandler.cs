/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

public sealed class InvoiceCommandHandler<TDbContext>
    : ICommandHandler<Invoice>
    where TDbContext : DbContext
{
    private ILogger<InvoiceCommandHandler<TDbContext>> _logger;

    public InvoiceCommandHandler(ILogger<InvoiceCommandHandler<TDbContext>> logger)
        => _logger = logger;

    public ValueTask<CommandResult> ExecuteAsync(CommandRequest<Invoice> request)
    {
        var message = $"You are not allowed to update an Invoice outside the Invoice Aggregate context";
        _logger.LogError(message);
        return ValueTask.FromResult(CommandResult.Failure(message));
    }
}
