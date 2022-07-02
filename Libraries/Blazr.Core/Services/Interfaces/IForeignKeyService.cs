/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Core;

public interface IForeignKeyService<TFkRecord, TService>
    where TFkRecord : class, IFkListItem, new()
    where TService : class
{
    public IEnumerable<IFkListItem> Items { get; }

    public ValueTask<bool> GetFkList();
}
