using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nest;
using NitelikliBilisim.App.Extensions;
using NitelikliBilisim.Core.ComplexTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

namespace NitelikliBilisim.App.Filters
{
    public class UserLoggerFilterAttribute : Attribute, IActionFilter
    {

        private static readonly ConnectionSettings connSettings =
       new ConnectionSettings(new Uri("http://localhost:9200/"))
                       .DefaultIndex("transactionlog")
                       .DefaultMappingFor<TransactionLog>(m => m
                       .IndexName("transactionlog")
                       .IdProperty(p => p.Id)

           );
        private static readonly ElasticClient elasticClient = new ElasticClient(connSettings);

        private List<TransactionLog> Search(string controllerName)
        {
            //Tek bir get isteği
            //var response2 = elasticClient.Get<TransactionLog>("1aa2a42c-0f96-3f29-f76a-82d021b47034", x => x.Index("transactionlog"));

            //Search İşlemi
            //var response = elasticClient.Search<TransactionLog>(i => i
            //.Query(x => x.Match(m => m.Field(f => f.ControllerName).Query("educationgain"))));

            //Search string value
            var response3 = elasticClient.Search<TransactionLog>(i => i.Query(m => m.Match(x => x.Field(p => p.ControllerName).Query(controllerName))));


            List<TransactionLog> items = new List<TransactionLog>();
            foreach (var item in response3.Documents)
                items.Add(item);
            return items;
        }


        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine(DateTime.Now);
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //Kullanıcıya bir sessionId veriyoruz
            if (context.HttpContext.Session.GetString("userSessionId") == null)
            {
                context.HttpContext.Session.SetString("userSessionId", Guid.NewGuid().ToString());
            }

            //Controller ve Action name için ActionDescriptor seçiliyor.
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            //Admin işlemleri log dışı tutuluyor.
            if (context.HttpContext.User.FindFirstValue(ClaimTypes.Role) != "Admin")
            {
                #region Create and Insert TransactionLog
                TransactionLog log = new TransactionLog
                {
                    ControllerName = descriptor.ControllerName,
                    ActionName = descriptor.ActionName,
                    ActionArguments = context.ActionArguments,
                    SessionId = context.HttpContext.Session.GetString("userSessionId"),
                    IpAddress = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                    UserId = context.HttpContext.User.Identity.IsAuthenticated ? context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) : string.Empty
                };
                //Nesne es ye insert ediliyor.
                elasticClient.Index<TransactionLog>(log, idx => idx.Index("transactionlog"));
                #endregion

            }
        }

    }
}
