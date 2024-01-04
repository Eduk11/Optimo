using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Log.ILog
{
    public interface ILoggerAdapter<T>
    {
        public void LogInformation(string message, params object[] args);
        public void LogWarningM(string message, params object[] args);
        public void LogWarningE(Exception ex, string message, params object[] args);
        public void LogDebug(string message, params object[] args);
        public void LogError(string message, params object[] args);
        public void LogCriticalM(string message, params object[] args);
        public void LogCriticalE(Exception ex, string message, params object[] args);
    }
}
