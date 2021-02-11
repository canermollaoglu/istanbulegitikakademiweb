using Elasticsearch.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nest;
using Newtonsoft.Json;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Core.ESOptions.ESEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
            if (descriptor.ControllerName == "Blog" && descriptor.ActionName == "Detail")
            {
                if (context.ActionArguments["seoUrl"]!=null && context.ActionArguments["catSeoUrl"]!=null)
                {
                    var ipAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
                    string catSeoUrl = context.ActionArguments["catSeoUrl"].ToString();
                    string seoUrl = context.ActionArguments["seoUrl"].ToString();
                   var anyIp = AnyIpAddress(ipAddress,catSeoUrl,seoUrl);
                    if (!anyIp)
                    {
                        BlogViewLog bViewLog = new BlogViewLog
                        {
                            SeoUrl = context.ActionArguments["seoUrl"],
                            CatSeoUrl = context.ActionArguments["catSeoUrl"],
                            SessionId = context.HttpContext.Session.GetString("userSessionId"),
                            IpAddress = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                            UserId = context.HttpContext.User.Identity.IsAuthenticated ? context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) : null
                        };
                        CheckBlogViewLogIndex();
                        _elasticClient.IndexDocument(bViewLog);
                    }
                }
            }

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
                CheckLogIndex();
                //Nesne es ye insert ediliyor.
                var response = _elasticClient.IndexDocument(log);
                Console.WriteLine(response.IsValid);
                #endregion
            }
        }


        #region Helper Methods

        private bool AnyIpAddress(string ipAddress,string catSeoUrl, string seoUrl)
        {
            int count = 0;
            var counts = _elasticClient.Count<BlogViewLog>(s =>
            s.Query(
                q =>
                q.Term(t=>t.IpAddress,ipAddress) &&
                q.Term(t => t.CatSeoUrl, catSeoUrl) &&
                q.Term(t => t.SeoUrl, seoUrl)));
            if (counts.IsValid)
            {
                count = (int)counts.Count;
            }
            return count > 0 ? true : false;
        }

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
                .Query(q => q.Term(t => t.SessionId,sessionId) && q.Bool(b => b.MustNot(m => m.Exists(t => t.Field(f => f.UserId)))))
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
        private void CheckLogIndex()
        {
            var response = _elasticClient.Indices.Exists(ElasticSearchIndexNameUtility.TransactionLogIndex);
            if (!response.Exists)
            {
                _elasticClient.Indices.Create(ElasticSearchIndexNameUtility.TransactionLogIndex, index =>
                   index.Map<TransactionLog>(x => x.AutoMap()));
            }
        }

        private void CheckBlogViewLogIndex() {
            var response = _elasticClient.Indices.Exists(ElasticSearchIndexNameUtility.BlogViewLogIndex);
            if (!response.Exists)
            {
                _elasticClient.Indices.Create(ElasticSearchIndexNameUtility.BlogViewLogIndex, index =>
                   index.Map<BlogViewLog>(x => x.AutoMap()));
            }
        }

        /// <summary>
        /// Finds the MAC address of the NIC with maximum speed.
        /// </summary>
        /// <returns>The MAC address.</returns>
        private string GetMacAddress()
        {
            const int MIN_MAC_ADDR_LENGTH = 12;
            string macAddress = string.Empty;
            long maxSpeed = -1;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                string tempMac = nic.GetPhysicalAddress().ToString();
                if (nic.Speed > maxSpeed &&
                    !string.IsNullOrEmpty(tempMac) &&
                    tempMac.Length >= MIN_MAC_ADDR_LENGTH)
                {
                    maxSpeed = nic.Speed;
                    macAddress = tempMac;
                }
            }

            return macAddress;
        }
        #endregion
    }
}
