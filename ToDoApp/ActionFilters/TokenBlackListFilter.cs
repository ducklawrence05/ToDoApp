using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace ToDoApp.ActionFilters
{
    public class TokenBlackListFilter : IAuthorizationFilter
    {
        private readonly IMemoryCache _cache;

        public TokenBlackListFilter(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader);
            //var accessToken = authorizationHeader.FirstOrDefault()?.Split(" ").Last();

            foreach (var claim in context.HttpContext.User.Claims)
            {
                Console.WriteLine($"Claim type: {claim.Type}, value: {claim.Value}");
            }


            var userId = context
                .HttpContext
                .User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (_cache.TryGetValue($"BLACK_LIST:{userId}", out _))
            {
                context.Result = new UnauthorizedResult(); // Token bị block
            }
        }
    }
}
