namespace CommandWrapper.Core.Exceptions;

public class NotValidArgumentException : ArgumentException
{
    public NotValidArgumentException()
        : base() { }
    
    public NotValidArgumentException(string message)
        : base(message) { }
    
    public NotValidArgumentException(string message, Exception innerException)
        : base(message, innerException) { }
    
    public NotValidArgumentException(string message, string paramName)
        : base(message, paramName) { }
    
    public NotValidArgumentException(string message, string paramName, Exception innerException)
        : base(message, paramName, innerException) { }
}