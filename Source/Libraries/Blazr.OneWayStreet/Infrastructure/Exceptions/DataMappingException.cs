/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.OneWayStreet.Infrastructure;

public class DataMappingException : Exception
{
    public DataMappingException() : base() { }
    public DataMappingException(string message) : base(message) { }
    public DataMappingException(string messge, Exception innnerException) : base(messge, innnerException) { }
}
