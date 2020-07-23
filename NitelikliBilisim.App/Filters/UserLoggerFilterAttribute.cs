using Elasticsearch.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nest;
using Newtonsoft.Json;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.ComplexTypes.TransactionLogModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;


namespace NitelikliBilisim.App.Filters
{
    public class UserLoggerFilterAttribute : Attribute, IActionFilter
    {
        private readonly IElasticClient _elasticClient;

        public UserLoggerFilterAttribute(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        /// <summary>
        /// Verilen sessionid ile yapılan işlemler listesini döndürür.
        /// </summary>
        /// <param name="sessionUserId"></param>
        /// <returns></returns>
        /// 
        private List<TransactionLog> SearchSessionTransaction(string sessionUserId)
        {
            //Search string value
            var response = _elasticClient.Search<TransactionLog>(i => i.Query(m => m.Match(x => x.Field(p => p.SessionId).Query(sessionUserId))));

            List<TransactionLog> items = new List<TransactionLog>();
            foreach (var item in response.Documents)
                items.Add(item);
            return items;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //Controller ve Action name için ActionDescriptor seçiliyor.
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            string currentUserRoleName = context.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            string sessionId = context.HttpContext.Session.GetString("userSessionId");
            //Kullanıcıya bir sessionId atıyoruz.
            if (sessionId == null)
            {
                context.HttpContext.Session.SetString("userSessionId", Guid.NewGuid().ToString());
            }
            //if (context.HttpContext.Request.Method.Equals("POST"))
            
            //Admin işlemleri log dışı tutuluyor.
            if (currentUserRoleName != "Admin")
            {
                #region Create and Insert TransactionLog
                TransactionLog log = new TransactionLog
                {
                    ControllerName = descriptor.ControllerName,
                    ActionName = descriptor.ActionName,
                    Parameters = GetParameters(context.ActionArguments),
                    SessionId = context.HttpContext.Session.GetString("userSessionId"),
                    IpAddress = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                    UserId = context.HttpContext.User.Identity.IsAuthenticated ? context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) : null
                };


                if (log.UserId != null && sessionId != null)
                {
                    UpdateTransactionLogsSetUserId(log.UserId, sessionId);
                }

                //Index kontrol ediliyor yoksa oluşturuluyor.
                CheckIndex();
                //Nesne es ye insert ediliyor.
                var response = _elasticClient.IndexDocument(log);
                Console.WriteLine(response.IsValid);
                #endregion
            }
        }


        #region Helper Methods
        /// <summary>
        /// ActionArguments içerisindeki parametreleri Liste halinde Log Parameter e map ederek döndürür.
        /// </summary>
        private List<LogParameter> GetParameters(IDictionary<string, object> actionArguments)
        {
            return actionArguments.Where(x=>x.Value!=null).Select(x => new LogParameter
            {
                ParameterName = x.Key,
                ParameterValue = JsonConvert.SerializeObject(x.Value),
                ParameterType = x.Value.GetType().FullName,
            }).ToList();
        }

        /// <summary>
        /// Kullanıcı giriş yaptığında sahip olduğu sessionId ile tutulan ve userId alanı boş olan tüm loglara userId import ediliyor.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        private bool UpdateTransactionLogsSetUserId(string userId, string sessionId)
        {
            var response = _elasticClient.UpdateByQuery<TransactionLog>(u => u
                .Query(q => q.Match(m => m.Field(f => f.SessionId).Query(sessionId)) && q.Bool(b => b.MustNot(m => m.Exists(t => t.Field(f => f.UserId)))))
                .Script(s => s.
                Source("ctx._source.userId = params.userId")
                .Lang(ScriptLang.Painless)
                .Params(p => p.Add("userId", userId)))
                .Conflicts(Conflicts.Proceed)
                .Refresh(true)
              );
            return response.IsValid;
        }
        /// <summary>
        /// Indıcesin varlığını kontrol edip yoksa oluşturuyor.
        /// </summary>
        private void CheckIndex()
        {
            var response = _elasticClient.Indices.Exists("usertransactionlogsindex");
            if (!response.Exists)
            {
                _elasticClient.Indices.Create("usertransactionlogsindex", index =>
                   index.Map<TransactionLog>(x => x.AutoMap()));
            }
        }

        private string GetMacAddress(ActionExecutingContext context)
        {
            string macAddress = string.Empty;
            macAddress = context.HttpContext.Request.Headers["X-Forwarded-For"].ToString();
            if (string.IsNullOrEmpty(macAddress))
            {
                foreach (var item in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                {
                    System.Net.NetworkInformation.OperationalStatus ot = item.OperationalStatus;
                    if (item.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                    {
                        macAddress = item.GetPhysicalAddress().ToString();
                        break;
                    }
                }
            }

            return macAddress;
        }
        #endregion
    }
}
