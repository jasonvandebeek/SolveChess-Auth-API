
using System.Runtime.Serialization;

namespace SolveChess.Logic.Exceptions;

[Serializable]
public class SolveChessBaseException : Exception
{

    public SolveChessBaseException()
    {
    }

    public SolveChessBaseException(string? message) : base(message)
    {
    }

    public SolveChessBaseException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected SolveChessBaseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

}

