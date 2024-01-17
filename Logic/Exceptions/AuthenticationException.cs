using System.Runtime.Serialization;

namespace SolveChess.Logic.Exceptions;

[Serializable]
public class AuthenticationException : SolveChessBaseException
{
    public AuthenticationException()
    {
    }

    public AuthenticationException(string? message) : base(message)
    {
    }

    public AuthenticationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected AuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}