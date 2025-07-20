using Microsoft.AspNetCore.Mvc.Filters;

namespace ToDoApp.ActionFilters
{
    public class TestFilter : IActionFilter, IResultFilter
    {
        // sau khi endpoint đã trả về result
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine("OnActionExecuted");
        }

        // trước khi endpoint được execute
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("OnActionExecuting");
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            Console.WriteLine("OnResultExecuted");
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            Console.WriteLine("OnResultExecuting");
        }
    }
}
