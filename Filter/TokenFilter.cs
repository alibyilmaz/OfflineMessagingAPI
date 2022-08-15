using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OfflineMessagingAPI.Helper
{
    public class TokenFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // do something after the action executes
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            string token = context.HttpContext.Request.Headers["Authorization"].ToString();
            Console.WriteLine(token);
        }
    }
}
