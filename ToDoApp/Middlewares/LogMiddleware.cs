namespace ToDoApp.Middlewares
{
    public class LogMiddleware : IMiddleware
    {
        private readonly ILogger<LogMiddleware> _logger;

        public LogMiddleware(ILogger<LogMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while processing the request.");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("An unexpected error occurred.");
            }
            //finally
            //{
            //    var request = context.Request;
            //    var response = context.Response;
            //    _logger.LogInformation($"Request: {request.Method} {request.Path}");
            //    //_logger.LogInformation("Request: {0} {1}", request.Method, request.Path);
            //    //_logger.LogInformation("Request: " + request.Method + " " + request.Path);
            //    _logger.LogInformation("Response: " + response.StatusCode);
            //}
        }
    }
}
