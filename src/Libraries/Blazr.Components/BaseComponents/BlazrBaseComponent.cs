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

using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;

namespace Blazr.Components;

public abstract class BlazrBaseComponent
{
    private RenderHandle _renderHandle;
    private RenderFragment _content;
    private bool _renderPending;
    private bool _hasNeverRendered = true;


    protected bool Initialized;
    protected bool NotInitialized => !this.Initialized;

    /// <summary>
    /// Frame/Layout/Wrapper Content that will be render if set
    /// </summary>
    protected virtual RenderFragment? Frame { get; set; }

    /// <summary>
    /// Razor Compiled content of BuildRenderTree.
    /// </summary>
    protected RenderFragment Body { get; init; }

    /// <summary>
    /// Unique Id that can be used to identifiy this instance of the component
    /// </summary>
    public Guid ComponentUid { get; init; } = Guid.NewGuid();

    public BlazrBaseComponent()
    {
        this.Body = (builder) => this.BuildRenderTree(builder);

        _content = (builder) =>
        {
            _renderPending = false;
            _hasNeverRendered = false;
            if (Frame is not null)
                Frame.Invoke(builder);
            else
                BuildRenderTree(builder);

            this.Initialized = true;
        };
    }

    public void Attach(RenderHandle renderHandle)
        => _renderHandle = renderHandle;

    /// <summary>
    /// Renders the component to the supplied <see cref="RenderTreeBuilder"/>.
    /// </summary>
    /// <param name="builder">A <see cref="RenderTreeBuilder"/> that will receive the render output.</param>
    protected virtual void BuildRenderTree(RenderTreeBuilder builder) { }

    /// <summary>
    /// Calls StateHasChanged and ensures it is applied immediately
    /// by yielding and giving the Renderer thread time to run.
    /// </summary>
    /// <returns></returns>
    public async Task RenderAsync()
    {
        this.StateHasChanged();
        await Task.Yield();
    }

    public void StateHasChanged()
    {
        if (_renderPending)
            return;

        var shouldRender = _hasNeverRendered || this.ShouldRender() || _renderHandle.IsRenderingOnMetadataUpdate;

        if (shouldRender)
        {
            _renderPending = true;
            _renderHandle.Render(_content);
        }
    }

    protected virtual bool ShouldRender() => true;

    protected Task InvokeAsync(Action workItem)
        => _renderHandle.Dispatcher.InvokeAsync(workItem);

    protected Task InvokeAsync(Func<Task> workItem)
        => _renderHandle.Dispatcher.InvokeAsync(workItem);
}