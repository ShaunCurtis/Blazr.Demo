/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public interface IListQueryHandler<TRecord>
    : IHandler<IListQuery<TRecord>, ValueTask<ListProviderResult<TRecord>>>
    where TRecord : class, new()
{}
