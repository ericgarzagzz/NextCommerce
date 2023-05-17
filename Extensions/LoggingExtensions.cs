using FluentEmail.Core.Models;

namespace NextCommerce.Extensions
{
    public static class LoggingExtensions
    {
        public static void Log(this ILogger logger, LogLevel logLevel, string emailType, SendResponse emailSendResponse) =>
            logger.Log(logLevel, "Failed to send email {}. Error Messages: {}", emailType, string.Join(", ", emailSendResponse.ErrorMessages));

        public static void LogError(this ILogger logger, string emailType, SendResponse emailSendResponse) => Log(logger, LogLevel.Error, emailType, emailSendResponse);
    }
}
