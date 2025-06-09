using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToDoApp.Constant;

namespace ToDoApp.Application.ActionFilters
{
    public class AuthorizationFilter : ActionFilterAttribute, IAuthorizationFilter
    {
        private readonly string _allowedRoles;

        public AuthorizationFilter(string allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserId");
            
            if (userId == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            
            var role = context.HttpContext.Session.GetString("Role");

            var allowedRoles = _allowedRoles.Split(',');

            if (!allowedRoles.Contains(role))
            {
                context.Result = new StatusCodeResult(403);
            }

        }
    }
}
