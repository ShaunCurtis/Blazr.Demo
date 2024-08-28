using Blazr.FluxGate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Blazr.App.Core;

public static class FluxGateExtensions
{
    public static CommandState ToCommandState(this FluxGateState state)
    {
        if (state.IsNew)
            return CommandState.Add;
        if (state.IsDeleted)
            return CommandState.Delete;
        if (state.IsModified)
            return CommandState.Update;

        return CommandState.None;
    }

    public static IDataResult ToDataResult<TFluxItem>(this FluxGateResult<TFluxItem> result)
        => DataResult.Create(result.Success, result.Message);
}
