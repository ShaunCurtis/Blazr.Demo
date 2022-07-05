/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public interface IDataBroker
{
    public ValueTask<ListProviderResult<TRecord>> GetRecordsAsync<TRecord>(ListProviderRequest<TRecord> request) where TRecord: class, new();

    public ValueTask<RecordProviderResult<TRecord>> GetRecordAsync<TRecord>(Guid id) where TRecord : class, new();

    public ValueTask<FKListProviderResult> GetFKListAsync<TRecord>() where TRecord : class, IFkListItem, new();

    public ValueTask<RecordCountProviderResult> GetRecordCountAsync<TRecord>() where TRecord : class, new();

    public ValueTask<CommandResult> AddRecordAsync<TRecord>(TRecord record) where TRecord : class, new();

    public ValueTask<CommandResult> UpdateRecordAsync<TRecord>(TRecord record) where TRecord : class, new();

    public ValueTask<CommandResult> DeleteRecordAsync<TRecord>(TRecord record) where TRecord : class, new();
}
