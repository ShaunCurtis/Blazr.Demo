/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Core;

/// <summary>
/// The purpose of the `ICommandEntity` interface is to identify Entity objects 
/// that the data pipeline can update.
/// No `ICommandEntity`, you get an exception if you try and run an update against the data store
/// </summary>
public interface ICommandEntity 
{ 
}
