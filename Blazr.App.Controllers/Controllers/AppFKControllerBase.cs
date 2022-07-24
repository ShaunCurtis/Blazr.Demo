/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Controllers;

[ApiController]
public abstract class AppFKControllerBase<TRecord, TFKRecord>
    : ControllerBase
    where TRecord : class, new()
    where TFKRecord : class, IFkListItem, new()
{
    private ICQSDataBroker _dataBroker;

    public AppFKControllerBase(ICQSDataBroker dataBroker)
        => _dataBroker = dataBroker;

    [Mvc.Route("/api/[controller]/recordlistquery")]
    [Mvc.HttpPost]
    public async Task<ListProviderResult<TRecord>> RecordListQuery([FromBody] RecordListQuery<TRecord> query)
        => await _dataBroker.ExecuteAsync<TRecord>(query);

    [Mvc.Route("/api/[controller]/ifilteredlistquery")]
    [Mvc.HttpPost]
    public async Task<ListProviderResult<TRecord>> IFilteredListQuery([FromBody] IFilteredListQuery<TRecord> query)
        => await _dataBroker.ExecuteAsync<TRecord>(query);

    [Mvc.Route("/api/[controller]/recordguidkeyquery")]
    [Mvc.HttpPost]
    public async Task<RecordProviderResult<TRecord>> RecordGuidKeyQuery([FromBody] RecordGuidKeyQuery<TRecord> query)
        => await _dataBroker.ExecuteAsync<TRecord>(query);

    [Mvc.Route("/api/[controller]/fklistquery")]
    [Mvc.HttpPost]
    public async Task<FKListProviderResult> FKListQuery([FromBody] FKListQuery<TFKRecord> query)
        => await _dataBroker.ExecuteAsync<TFKRecord>(query);

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