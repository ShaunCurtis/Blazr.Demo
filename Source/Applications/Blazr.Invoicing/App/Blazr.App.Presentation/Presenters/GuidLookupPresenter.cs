/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public interface IGuidLookUpPresenter<TItem>
    where TItem : class, IGuidLookUpItem, new()
{
    public Task LoadTask { get; }

    public IEnumerable<TItem> Items { get; }

    public Task<bool> LoadAsync();
}


public class GuidLookUpPresenter<TItem>
    : IGuidLookUpPresenter<TItem>
        where TItem : class, IGuidLookUpItem, new()
{
    protected IDataBroker DataBroker;

    public Task LoadTask { get; private set; } = Task.CompletedTask;

    public IEnumerable<TItem> Items { get; protected set; } = Enumerable.Empty<TItem>();

    public GuidLookUpPresenter(IDataBroker dataBroker)
    {
        DataBroker = dataBroker;
        LoadTask = LoadAsync();
    }

    public async Task<bool> LoadAsync()
    {
        var result = await this.DataBroker.ExecuteQueryAsync<TItem>(new ListQueryRequest());
        this.Items = result.Items;

        return result.Successful;
    }

    public async void OnUpdate(object? sender, EventArgs e)
        => await this.LoadAsync();
}

