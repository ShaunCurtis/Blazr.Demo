/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

/// <summary>
/// Defines the interface for all View Presenters
/// </summary>
/// <typeparam name="TRecord">Record to display</typeparam>
/// <typeparam name="TKey">The record entity key type</typeparam>
public interface IViewPresenter<TRecord, TKey>
    where TRecord : class, new()
    where TKey : IEntityKey
{
    public IDataResult LastDataResult { get; }
    public TRecord Item { get; }

    public Task LoadAsync(TKey id);
}
