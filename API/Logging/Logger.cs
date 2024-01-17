
using SolveChess.API.Exceptions;

namespace SolveChess.API.Logging;

public static class Logger
{

    private static readonly string logDirectory = Environment.GetEnvironmentVariable("SolveChess_LogPath") ?? throw new MissingEnvVariableException("No path for log files found in .env variables!");
    private static readonly string logFileName = $"{DateTime.Now:yyyy/MM/dd}.solvechess.log";
    private static readonly string logFilePath = Path.Combine(logDirectory, logFileName);

    public static void WriteLineToLogFile(string line)
    {
        if (!Directory.Exists(logDirectory))
            Directory.CreateDirectory(logDirectory);

        using StreamWriter sw = File.AppendText(logFilePath);
        sw.WriteLine(line);
    }

    public static void LogException(Exception exception)
    {
        var log = $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] {exception.GetType().Name}: {exception.Message}";

        Console.WriteLine(log);
        WriteLineToLogFile(log);
    }

}
