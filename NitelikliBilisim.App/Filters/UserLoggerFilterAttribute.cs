using Microsoft.AspNetCore.Mvc.Filters;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using System;

namespace NitelikliBilisim.App.Filters
{
    public class UserLoggerFilterAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine(DateTime.Now);
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine(DateTime.Now);

            UserLogModel log = new UserLogModel();
        }
    }
}
