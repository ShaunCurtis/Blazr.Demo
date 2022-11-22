/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Core.Edit;

/// <summary>
/// The purpose of IRecordEditContext is to define the Record specific properties and methods
/// that Record edit contexts must implement for genric edit form base classes to interact with them. 
/// </summary>
/// <typeparam name="TRecord">Base data record for the context</typeparam>
public interface IRecordEditContext<TRecord> : IEditContext
    where TRecord : class, new()
{
    public TRecord Record { get; }

    public void Load(TRecord record, bool notify = true);

    public TRecord CleanRecord { get; }

    public TRecord AsNewRecord();

    public void Reset();
}