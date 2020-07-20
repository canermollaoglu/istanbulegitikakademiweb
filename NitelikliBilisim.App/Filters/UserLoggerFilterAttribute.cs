using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nest;
using NitelikliBilisim.Core.ComplexTypes;
using System;
using System.Collections.Generic;
using System.Security.Claims;


namespace NitelikliBilisim.App.Filters
{
    public class UserLoggerFilterAttribute : Attribute, IActionFilter
    {

        //Bu alanda kullanıcı adı, şifre index adı vb. configurasyonlar yapılacak.
        private static readonly ConnectionSettings connSettings =
       new ConnectionSettings(new Uri("http://localhost:9200/"))
                       .DefaultIndex("transactionlog")
                       .DefaultMappingFor<TransactionLog>(m => m
                       .IndexName("transactionlog")
                       .IdProperty(p => p.Id)

           );
        private static readonly ElasticClient elasticClient = new ElasticClient(connSettings);

        /// <summary>
        /// Verilen sessionid ile yapılan işlemler listesini döndürür.
        /// </summary>
        /// <param name="sessionUserId"></param>
        /// <returns></returns>
        private List<TransactionLog> SearchSessionTransaction(string sessionUserId)
        {
            //Search string value
            var response = elasticClient.Search<TransactionLog>(i => i.Query(m => m.Match(x => x.Field(p => p.SessionId).Query(sessionUserId))));


            List<TransactionLog> items = new List<TransactionLog>();
            foreach (var item in response.Documents)
                items.Add(item);
            return items;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine(DateTime.Now);
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //Kullanıcıya bir sessionId atıyoruz.
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
