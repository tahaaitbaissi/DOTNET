using System;
using System.IO;

namespace CarRental.Desktop.Services
{
    public static class FileLogger
    {
        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "desktop_debug.log");
        private static readonly object LockObj = new object();

        public static void Log(string message)
        {
            try
            {
                lock (LockObj)
                {
                    var entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [INFO] {message}{Environment.NewLine}";
                    File.AppendAllText(LogFilePath, entry);
                }
            }
            catch { /* Ignore logging errors to prevent app crash */ }
        }

        public static void LogError(string message, Exception? ex = null)
        {
            try
            {
                lock (LockObj)
                {
                    var entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [ERROR] {message}";
                    if (ex != null)
                    {
                        entry += $"{Environment.NewLine}{ex}";
                    }
                    entry += Environment.NewLine;
                    File.AppendAllText(LogFilePath, entry);
                }
            }
            catch { /* Ignore */ }
        }
    }
}
