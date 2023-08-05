/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet;

/// <summary>
/// Exception raised when the state already exists in the store
/// </summary>
public class EntityAlreadyExistsException : Exception
{
    public EntityAlreadyExistsException(string message) : base(message)
    { }
}
