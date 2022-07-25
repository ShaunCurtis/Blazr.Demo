/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Controllers;

[ApiController]
public abstract class AppControllerBase<TRecord>
    : ControllerBase
    where TRecord : class, new()
{
    private ICQSDataBroker _dataBroker;

    public AppControllerBase(ICQSDataBroker dataBroker)
        => _dataBroker = dataBroker;

    [Mvc.Route("/api/[controller]/listquery")]
    [Mvc.HttpPost]
    public async Task<ListProviderResult<TRecord>> ListQuery([FromBody] ListQuery<TRecord> query)
        => await _dataBroker.ExecuteAsync<TRecord>(query);

    [Mvc.Route("/api/[controller]/ilistquery")]
    [Mvc.HttpPost]
    public async Task<ListProviderResult<TRecord>> IListQuery([FromBody] IListQuery<TRecord> query)
        => await _dataBroker.ExecuteAsync<TRecord>(query);

    [Mvc.Route("/api/[controller]/recordquery")]
    [Mvc.HttpPost]
    public async Task<RecordProviderResult<TRecord>> RecordGuidKeyQuery([FromBody] RecordQuery<TRecord> query)
        => await _dataBroker.ExecuteAsync<TRecord>(query);

    [Mvc.Route("/api/[controller]/addrecordcommand")]
    [Mvc.HttpPost]
    public async Task<CommandResult> AddRecordCommand([FromBody] AddRecordCommand<TRecord> command)
        => await _dataBroker.ExecuteAsync<TRecord>(command);

    [Mvc.Route("/api/[controller]/updaterecordcommand")]
    [Mvc.HttpPost]
    public async Task<CommandResult> UpdateRecordCommand([FromBody] UpdateRecordCommand<TRecord> command)
        => await _dataBroker.ExecuteAsync<TRecord>(command);

    [Mvc.Route("/api/[controller]/deleterecordcommand")]
    [Mvc.HttpPost]
    public async Task<CommandResult> DeleteRecordCommand([FromBody] DeleteRecordCommand<TRecord> command)
        => await _dataBroker.ExecuteAsync<TRecord>(command);
}