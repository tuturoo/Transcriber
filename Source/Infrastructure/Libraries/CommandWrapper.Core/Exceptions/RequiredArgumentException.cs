using CommandWrapper.Core.Abstractions;

namespace CommandWrapper.Core.Exceptions;

public class RequiredArgumentException : Exception
{
    public RequiredArgumentException()
    {
    }

    public RequiredArgumentException(string message) : base(message)
    {
    }

    public RequiredArgumentException(string message, Exception inner) : base(message, inner)
    {
    }

    public CommandArgument? Argument { get; init; }
}