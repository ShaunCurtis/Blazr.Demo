/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

public sealed class InvoiceItemCommandHandler<TDbContext>
    : ICommandHandler<InvoiceItem>
    where TDbContext : DbContext
{
    private ILogger<InvoiceItemCommandHandler<TDbContext>> _logger;

    public InvoiceItemCommandHandler(ILogger<InvoiceItemCommandHandler<TDbContext>> logger)
        => _logger = logger;

    public ValueTask<CommandResult> ExecuteAsync(CommandRequest<InvoiceItem> request)
    {
        var message = $"You are not allowed to update an InvoiceItem outside the Invoice Aggregate context";
        _logger.LogError(message);
        return ValueTask.FromResult(CommandResult.Failure(message));
    }
}
