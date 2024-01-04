using Library.Log.ILog;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Log
{
    public class LoggerAdapter<T> : ILoggerAdapter<T>
    {
        private readonly ILogger<T> _logger;

        public LoggerAdapter(ILogger<T> logger) { _logger = logger; }
        public void LogWarningE(Exception ex, string message, params object[] args)
        {
            _logger.LogError(ex, message, args);
        }
        public void LogWarningM(string message, params object[] args)
        {
            _logger.LogWarning("{message}", message);
            ////_logger.LogError(message, args);
        }
        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation("{message}", message);
            ////_logger.LogInformation(message, args);
        }
        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug("{message}", message);
            ////_logger.LogDebug(message, args);
        }
        public void LogError(string message, params object[] args)
        {
            _logger.LogWarning("{message}", message);
            ////_logger.LogWarning(message, args);
        }
        public void LogCriticalM(string message, params object[] args)
        {
            ////_logger.LogCritical(message, args);
            _logger.LogCritical("{message}", message);
        }

        public void LogCriticalE(Exception ex, string message, params object[] args)
        {
            _logger.LogCritical(ex, message, args);
        }
    }
}
