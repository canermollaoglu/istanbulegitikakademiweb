using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using NitelikliBilisim.Core.ComplexTypes;
using System;
using System.Security.Claims;
using Serilog;

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
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            TransactionLog log = new TransactionLog
            {
                ControllerName = descriptor.ControllerName,
                ActionName = descriptor.ActionName,
                SessionId = context.HttpContext.Session.Id,
                IpAddress = context.HttpContext.Connection.RemoteIpAddress.ToString()
            };
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                log.UserId = userId;
            }
            Log.Information("Processed {@log} in {Time} ms", log, DateTime.Now);

        }
    }
}
