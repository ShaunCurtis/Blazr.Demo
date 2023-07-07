/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core;

public readonly record struct ItemQueryRequest( Guid Uid, CancellationToken Cancellation = new ());
