//============================================================
//   Author: Shaun Curtis, Cold Elm Coders
//   License: Use And Donate
//   If you use it, donate something to a charity somewhere
//
//   Code contains sections from ComponentBase in the ASPNetCore Repository
//   https://github.com/dotnet/aspnetcore/blob/main/src/Components/Components/src/ComponentBase.cs
//
//   Original Licence:
//
//   Licensed to the .NET Foundation under one or more agreements.
//   The .NET Foundation licenses this file to you under the MIT license.
//============================================================

using Microsoft.AspNetCore.Components;

namespace Blazr.Components;

public class BlazrUIBase : BlazrBaseComponent, IComponent
{
    public Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);
        this.StateHasChanged();
        return Task.CompletedTask;
    }
}
