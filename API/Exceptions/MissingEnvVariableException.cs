using SolveChess.Logic.Exceptions;
using System.Runtime.Serialization;

namespace SolveChess.API.Exceptions;

[Serializable]
public class MissingEnvVariableException : SolveChessBaseException
{
    public MissingEnvVariableException()
    {
    }

    public MissingEnvVariableException(string? message) : base(message)
    {
    }

    public MissingEnvVariableException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected MissingEnvVariableException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
