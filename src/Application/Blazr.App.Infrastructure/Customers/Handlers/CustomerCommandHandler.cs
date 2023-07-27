/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

public sealed class CustomerCommandHandler<TDbContext>
    : ICommandHandler<Customer>
    where TDbContext : DbContext
{
    private ILogger<CustomerCommandHandler<TDbContext>> _logger;
    private readonly ICommandHandler _commandHandler;
    private readonly IDboEntityMap<DboCustomer, Customer> _mapper;

    public CustomerCommandHandler(ILogger<CustomerCommandHandler<TDbContext>> logger, ICommandHandler commandHandler, IDboEntityMap<DboCustomer, Customer> mapper)
    {
        _logger = logger;
        _commandHandler = commandHandler;
        _mapper = mapper;
    }

    public ValueTask<CommandResult> ExecuteAsync(CommandRequest<Customer> request)
    {
        var dbo = _mapper.Map(request.Item);
        if (dbo is null)
        {
            var message = $"Could not map {request.Item.GetType()} to it's Dbo object.";
            _logger.LogError(message);
            return ValueTask.FromResult(CommandResult.Failure(message));
        }
        var newRequest = new CommandRequest<DboCustomer>(dbo);
        return  _commandHandler.ExecuteAsync(newRequest);
    }
}
