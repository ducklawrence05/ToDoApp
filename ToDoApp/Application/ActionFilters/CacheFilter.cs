using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using ToDoApp.Application.Services.CacheService;

namespace ToDoApp.Application.ActionFilters
{
    public class CacheFilter : ActionFilterAttribute
    {
        //private readonly ICacheService _cacheService;
        private readonly IMemoryCache _cache;
        private readonly int _duration;
        private string key;

        public CacheFilter(
            //ICacheService cacheService, 
            IMemoryCache cache,
            int duration)
        {
            //_cacheService = cacheService;
            _cache = cache;
            _duration = duration;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            key = context.HttpContext.Request.Path.ToString();

            if (_cache.TryGetValue(key, out var cacheData))
            {
                context.Result = new ObjectResult(cacheData);
                return;
            }

            // Cách 2:
            //var cacheData = _cache.Get(key);
            //if (cacheData != null)
            //{
            //    context.Result = new ObjectResult(cacheData);
            //}

            //var cacheData = _cacheService.Get(key);
            //if (cacheData != null)
            //{
            //    context.Result = new ObjectResult(cacheData.Value);
            //}
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine("CACHE SETTTTTTT");
            var data = context.Result as ObjectResult;

            if (data != null)
            {
                //_cacheService.Set(key, data.Value, _duration);
                _cache.Set(key, data.Value, TimeSpan.FromSeconds(_duration));
            }
        }
    }
}
