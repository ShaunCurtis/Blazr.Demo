/// Lighlty Modified version of OwningComponentBase
/// Ogiginal Licence
/// 
/// Licensed to the .NET Foundation under one or more agreements.
/// The .NET Foundation licenses this file to you under the MIT license.

/// ============================================================
/// Mods Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI;

public abstract class BlazrOwningComponentBase : BlazrComponentBase, IDisposable
{
    private AsyncServiceScope? _scope;
    protected bool IsDisposed { get; private set; }

    [Inject] IServiceScopeFactory ScopeFactory { get; set; } = default!;


    protected IServiceProvider ScopedServices
    {
        get
        {
            if (ScopeFactory == null)
                throw new InvalidOperationException("Services cannot be accessed before the component is initialized.");

            if (this.IsDisposed)
                throw new ObjectDisposedException($"{this.GetType().Name}");

            _scope ??= ScopeFactory.CreateAsyncScope();
            return _scope.Value.ServiceProvider;
        }
    }

    void IDisposable.Dispose()
    {
        if (!IsDisposed)
        {
            _scope?.Dispose();
            _scope = null;
            Dispose(disposing: true);
            IsDisposed = true;
        }
    }

    protected virtual void Dispose(bool disposing) { }
}

public abstract class BlazrOwningComponentBase<TService> : BlazrOwningComponentBase, IDisposable where TService : notnull
{
    private TService _item = default!;

    protected TService Service
    {
        get
        {
            if (this.IsDisposed)
                throw new ObjectDisposedException($"{this.GetType().Name}");

            _item ??= ScopedServices.GetRequiredService<TService>();
            return _item;
        }
    }
}