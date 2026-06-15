namespace TraineeManagementApi.Helper;

public class CustomFileLoggerProvider : ILoggerProvider
{
    private readonly string _filePath;

    public CustomFileLoggerProvider(string filePath) => _filePath = filePath;

    public ILogger CreateLogger(string categoryName) => new CustomFileLogger(_filePath);

    public void Dispose() { }
}
