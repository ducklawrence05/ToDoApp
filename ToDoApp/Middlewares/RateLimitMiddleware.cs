
using System.Diagnostics;

namespace ToDoApp.Middlewares
{
    // trong 30s, he thong chi cho phep 10 request
    public class RateLimitMiddleware : IMiddleware
    {
        private int _limit;
        private Stopwatch _stopwatch;

        public RateLimitMiddleware()
        {
            _limit = 0;
            _stopwatch = Stopwatch.StartNew();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (_stopwatch.ElapsedMilliseconds >= 30000)
            {
                _stopwatch.Restart();
                _limit = 0;
            }

            if (_limit == 10)
            {
                throw new Exception("Rate limit hehe");
            }
            else
            {
                _limit++;
            }
            await next(context);
        }
    }
}
