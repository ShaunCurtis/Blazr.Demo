/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Core;

/// <summary>
/// The purpose of the `IEntityKey` interface is to identify value objects 
/// that define the key for domain entity objects.
/// The data pipeline used this to build generic item queries.
/// </summary>
public interface IEntityKey 
{ 
    public object KeyValue { get; }
}
