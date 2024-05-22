/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class CustomerViewPresenter
{
    private readonly IItemRequestHandler<DmoCustomer> _itemRequestHandler;
    public IDataResult LastDataResult { get; private set; } = DataResult.Success();
    public DmoCustomer Item { get; private set; } = new();

    public CustomerViewPresenter(IItemRequestHandler<DmoCustomer> itemRequestHandler)
    {
        _itemRequestHandler = itemRequestHandler;
    }

    public async Task LoadAsync(CustomerId id)
    {
        var request = ItemQueryRequest.Create(id.Value);
        var result = await _itemRequestHandler.ExecuteAsync(request);
        LastDataResult = result;
        this.Item = result.Item ?? new();
    }
}
