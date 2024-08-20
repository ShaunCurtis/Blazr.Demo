/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode;

namespace Blazr.OneWayStreet.Core;

public static class DiodeExtensions
{
    public static IDataResult ToDataResult(this DiodeResult result)
        => result.Successful ? DataResult.Success() : DataResult.Failure(result.Message ?? "No Error Message Posted.");

    public static CommandState ToCommandState(this DiodeState state)
        => CommandState.GetState(state.Index);
}
