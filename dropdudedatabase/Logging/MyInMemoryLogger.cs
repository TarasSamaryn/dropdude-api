using System.Collections.Concurrent;

namespace MinefieldServer.Logging
{
    public class MyInMemoryLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new MyInMemoryLogger();
        }

        public void Dispose() { }
    }

    public class MyInMemoryLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        private static readonly ConcurrentQueue<string> _logs = new();

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var message = $"{DateTime.Now:HH:mm:ss} [{logLevel}] {formatter(state, exception)}";
            _logs.Enqueue(message);

            while (_logs.Count > 1000)
                _logs.TryDequeue(out _);
        }

        public static string[] GetLogs() => _logs.ToArray();
    }
}