using System;
using System.IO;

namespace KaraDownloader
{
    /// <summary>
    /// Simple file-based logger for troubleshooting.
    /// Writes timestamped log entries to KaraDownloader_log.txt
    /// in the same folder as the executable.
    /// </summary>
    public static class Logger
    {
        private static readonly string LogFilePath;
        private static readonly object _lock = new object();

        static Logger()
        {
            // Log file sits next to the .exe so the shop owner can find it easily
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            LogFilePath = Path.Combine(exeDir, "KaraDownloader_log.txt");
        }

        /// <summary>
        /// Append an informational message
        /// </summary>
        public static void Info(string message)
        {
            Write("INFO", message);
        }

        /// <summary>
        /// Append a warning message
        /// </summary>
        public static void Warn(string message)
        {
            Write("WARN", message);
        }

        /// <summary>
        /// Append an error message (with optional exception details)
        /// </summary>
        public static void Error(string message, Exception? ex = null)
        {
            string full = message;
            if (ex != null)
            {
                full += $"\n  Exception Type : {ex.GetType().FullName}"
                      + $"\n  Exception Msg  : {ex.Message}"
                      + $"\n  StackTrace     : {ex.StackTrace}";

                // Include inner exception if present
                if (ex.InnerException != null)
                {
                    full += $"\n  Inner Exception: {ex.InnerException.GetType().FullName}"
                          + $"\n  Inner Msg      : {ex.InnerException.Message}";
                }
            }
            Write("ERROR", full);
        }

        /// <summary>
        /// Core write method – thread-safe, never throws
        /// </summary>
        private static void Write(string level, string message)
        {
            try
            {
                lock (_lock)
                {
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    string line = $"[{timestamp}] [{level}] {message}";
                    File.AppendAllText(LogFilePath, line + Environment.NewLine);
                }
            }
            catch
            {
                // Logger must NEVER crash the app
            }
        }

        /// <summary>
        /// Return the full path so we can show it to the user if needed
        /// </summary>
        public static string GetLogPath() => LogFilePath;
    }
}
