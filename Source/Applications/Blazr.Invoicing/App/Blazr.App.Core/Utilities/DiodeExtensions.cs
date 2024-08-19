/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public static class DiodeExtensions
{
    public static IDataResult AsDataResult(this DiodeResult result)
        => result.Successful ? DataResult.Success() : DataResult.Failure(result.Message ?? "No Error Message Posted.");

    public static CommandState AsCommandState(this DiodeState state)
        => CommandState.GetState(state.Index);
}
