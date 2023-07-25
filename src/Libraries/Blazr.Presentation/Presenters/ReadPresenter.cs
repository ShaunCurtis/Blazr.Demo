/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Presentation;

public sealed class ReadPresenter<TRecord> : IReadPresenter<TRecord>
    where TRecord : class, IEntity, new()
{
    private IDataBroker _dataBroker;
    public TRecord Item { get; private set; } = new TRecord();

    public ItemQueryResult<TRecord> LastResult { get; private set; } = ItemQueryResult<TRecord>.Success(new());

    public ReadPresenter(IDataBroker dataBroker)
        => _dataBroker = dataBroker;

    public async ValueTask LoadAsync(EntityUid id)
        => await GetItemAsync(new ItemQueryRequest { Uid = id });

    private async ValueTask GetItemAsync(ItemQueryRequest request)
    {
        LastResult = await _dataBroker.GetItemAsync<TRecord>(request);

        if (LastResult.Successful)
            this.Item = this.LastResult.Item ?? new TRecord();
    }
}