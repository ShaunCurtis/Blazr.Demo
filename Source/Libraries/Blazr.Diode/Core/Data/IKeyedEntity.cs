﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode.Core;

/// <summary>
/// The purpose of the `IKeyedEntity` interface is to identify Entity objects 
/// that the Command can return a key generated by the database
/// KeyValue is loosely typed and represents the key value
/// The consumer must convert the returned value to the appropriate type or use a string and parse the value
/// </summary>
public interface IKeyedEntity 
{ 
    public object KeyValue { get; }
}
