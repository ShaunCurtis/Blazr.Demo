/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Demo.Data;

public class AddRecordCommandHandlerBase<TRecord, TDbContext>
    : RecordCommandHandlerBase<AddRecordCommand<TRecord>, TRecord>
        where TDbContext : DbContext, IWeatherDbContext
{
    private IDbContextFactory<TDbContext> _factory;

    public AddRecordCommandHandlerBase(IDbContextFactory<TDbContext> _factory, IRecordCommand<TRecord> command)
        : base(command)
        => _factory = _factory;
    { }

    public override void ExecuteCommand()
    {
        if (command.Record is not null)
         this.dbContext.Add(this.command.Record);
    }
}
