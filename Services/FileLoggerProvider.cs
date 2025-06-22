using Microsoft.Extensions.Logging;
using System;
using System.IO;
namespace Alzheimer_Detection.Services
{

    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _filePath;

        public FileLoggerProvider(string filePath)
        {
            _filePath = filePath.Replace("{Date}", DateTime.Today.ToString("yyyy-MM-dd"));
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(_filePath);
        }

        public void Dispose() { }
    }

    public class FileLogger : ILogger
    {
        private readonly string _filePath;
        private readonly object _lock = new();

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null) return;

            var message = formatter(state, exception);
            if (string.IsNullOrWhiteSpace(message)) return;

            var log = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{logLevel}] {message}{Environment.NewLine}";

            lock (_lock)
            {
                File.AppendAllText(_filePath, log);
            }
        }
    }







}