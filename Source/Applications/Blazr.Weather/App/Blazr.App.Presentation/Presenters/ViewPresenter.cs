/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class ViewPresenter<TRecord> : IViewPresenter<TRecord>
    where TRecord : class, new()
{
    private readonly IDataBroker _dataBroker;
    public IDataResult LastDataResult { get; private set; } = DataResult.Success();
    public TRecord Item { get; private set; } = new();

    public ViewPresenter(IDataBroker dataBroker)
    {
        _dataBroker = dataBroker;
    }

    public async Task LoadAsync(object id)
    {
        // Get the actual value of the Id type
        if (id is IGuidKey entity)
            id = entity.Value;

        var request = ItemQueryRequest.Create(id);
        var result = await _dataBroker.ExecuteQueryAsync<TRecord>(request);
        LastDataResult = result;
        this.Item = result.Item ?? new();
    }
}
