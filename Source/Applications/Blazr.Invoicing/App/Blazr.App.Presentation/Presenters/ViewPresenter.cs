/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class ViewPresenter<TRecord, TKey> : IViewPresenter<TRecord, TKey>
    where TRecord : class, new()
{
    private readonly IDataBroker _dataBroker;
    private readonly ILogger<ViewPresenter<TRecord, TKey>> _logger;
    public IDataResult LastDataResult { get; private set; } = DataResult.Success();
    public TRecord Item { get; private set; } = new();

    internal ViewPresenter(IDataBroker dataBroker, ILogger<ViewPresenter<TRecord, TKey>> logger)
    {
        _dataBroker = dataBroker;
        _logger = logger;
    }

    internal async ValueTask LoadAsync(TKey id)
    {
        var request = ItemQueryRequest<TKey>.Create(id);
        var result = await _dataBroker.ExecuteQueryAsync<TRecord, TKey>(request);
        LastDataResult = result;

        if (!result.Successful)
            _logger.LogError(result.Message);

        this.Item = result.Item ?? new();
    }
}
