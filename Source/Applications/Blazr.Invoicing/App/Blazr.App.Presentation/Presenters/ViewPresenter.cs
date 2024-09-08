using System.Security.Principal;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class ViewPresenter<TRecord, TKey> : IViewPresenter<TRecord, TKey>
    where TRecord : class, new()
    where TKey : IEntityKey
{
    private readonly IDataBroker _dataBroker;
    public IDataResult LastDataResult { get; private set; } = DataResult.Success();
    public TRecord Item { get; private set; } = new();

    internal ViewPresenter(IDataBroker dataBroker)
    {
        _dataBroker = dataBroker;
    }

    internal async ValueTask LoadAsync(TKey id)
    {
        var request = ItemQueryRequest<TKey>.Create(id);
        var result = await _dataBroker.ExecuteQueryAsync<TRecord, TKey>(request);
        LastDataResult = result;
        this.Item = result.Item ?? new();
    }
}
