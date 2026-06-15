using Microsoft.Extensions.Logging;
using System.IO;

namespace TraineeManagementApi.Helper;

public class CustomFileLogger : ILogger
{
    private readonly string _filePath;
    private static readonly object _lock = new();

    public CustomFileLogger(string filePath) => _filePath = filePath;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    // Checks if the configuration allows this level
    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {formatter(state, exception)}";
        if (exception != null)
        {
            message += $"{Environment.NewLine}{exception}";
        }

        // Lock ensures thread safety across async requests
        lock (_lock)
        {
            File.AppendAllText(_filePath, message + Environment.NewLine);
        }
    }
}
