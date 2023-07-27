/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Infrastructure;

public sealed class ProductCommandHandler<TDbContext>
    : ICommandHandler<Product>
    where TDbContext : DbContext
{
    private ILogger<ProductCommandHandler<TDbContext>> _logger;
    private readonly ICommandHandler _commandHandler;
    private readonly IDboEntityMap<DboProduct, Product> _mapper;

    public ProductCommandHandler(ILogger<ProductCommandHandler<TDbContext>> logger, ICommandHandler commandHandler, IDboEntityMap<DboProduct, Product> mapper)
    {
        _logger = logger;
        _commandHandler = commandHandler;
        _mapper = mapper;
    }

    public ValueTask<CommandResult> ExecuteAsync(CommandRequest<Product> request)
    {
        var dbo = _mapper.Map(request.Item);
        if (dbo is null)
        {
            var message = $"Could not map {request.Item.GetType()} to it's Dbo object.";
            _logger.LogError(message);
            return ValueTask.FromResult(CommandResult.Failure(message));
        }
        var newRequest = new CommandRequest<DboProduct>(dbo);
        return  _commandHandler.ExecuteAsync(newRequest);
    }
}
