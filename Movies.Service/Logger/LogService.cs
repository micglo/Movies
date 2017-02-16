using NLog;

namespace Movies.Service.Logger
{
    public class LogService : ILogService
    {
        private static NLog.Logger _logger;

        public void Log(string errorMessage, string innerErrorMessage, string stackTrace)
        {
            _logger = LogManager.GetLogger("error");
            LogEventInfo logInfo = new LogEventInfo();
            logInfo.Properties["ErrorMessage"] = errorMessage;
            logInfo.Properties["InnerErrorMessage"] = innerErrorMessage;
            logInfo.Properties["StackTrace"] = stackTrace;
            _logger.Debug(logInfo);
        }
    }
}