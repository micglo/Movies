using System.Web.Http.ExceptionHandling;
using NLog;

namespace Movies.WebApi.Utility.Exception.GlobalException
{
    public class GlobalExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            var innerException = string.Empty;
            if (context.Exception.InnerException?.InnerException?.InnerException != null)
                innerException = context.Exception.InnerException.InnerException.InnerException.Message;

            if (context.Exception.InnerException?.InnerException != null)
                innerException = context.Exception.InnerException.InnerException.Message;

            if (context.Exception.InnerException != null)
                innerException = context.Exception.InnerException.Message;

            var logger = LogManager.GetCurrentClassLogger();
            var logInfo = new LogEventInfo();
            logInfo.Properties["ErrorMessage"] = context.Exception.Message;
            logInfo.Properties["InnerErrorMessage"] = innerException;
            logInfo.Properties["StackTrace"] = context.Exception.StackTrace;
            logger.Debug(logInfo);
        }
    }
}