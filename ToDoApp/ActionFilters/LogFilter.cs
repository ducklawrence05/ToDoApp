using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace ToDoApp.ActionFilters
{
    public class LogFilter : IExceptionFilter
    {
        private readonly ILogger<LogFilter> _logger;
        private readonly LogLevel _logLevel;

        public LogFilter(ILogger<LogFilter> logger, LogLevel logLevel)
        {
            _logger = logger;
            _logLevel = logLevel;
        }

        public void OnException(ExceptionContext context)
        {
            Console.WriteLine("OnException");
            var exception = context.Exception;

            var message = $"Exception: {exception.Message}";

            _logger.Log(_logLevel, message);

            context.Result = new ObjectResult(new
            {
                message = @"An error occured while processing your request.
                            Please contact admin for more information",
                error = exception.Message
            })
            {
                StatusCode = 500
            };
        }
    }
}
