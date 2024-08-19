/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode;

public delegate DiodeMutationResult<TRecord> DiodeMutationDelegate<TIdentity, TRecord>(DiodeContext<TIdentity, TRecord> item)
    where TRecord : class, IDiodeRecord<TIdentity>, new();
