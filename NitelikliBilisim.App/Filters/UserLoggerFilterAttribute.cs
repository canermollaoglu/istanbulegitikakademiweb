using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using NitelikliBilisim.Business.Repositories.MongoDbRepositories;
using NitelikliBilisim.Core.MongoOptions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;


namespace NitelikliBilisim.App.Filters
{
    public class UserLoggerFilterAttribute : Attribute, IActionFilter
    {
        private readonly BlogViewLogRepository _blogViewLog;
        private readonly TransactionLogRepository _transactionLog;
        private readonly CampaignLogRepository _campaignLog;


        public UserLoggerFilterAttribute(TransactionLogRepository transactionLog, CampaignLogRepository campaignLog, BlogViewLogRepository blogViewLog)
        {
            _blogViewLog = blogViewLog;
            _transactionLog = transactionLog;
            _campaignLog = campaignLog;
        }


        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            string currentUserRoleName = context.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            string sessionId = context.HttpContext.Session.GetString("userSessionId");
            //Kullanıcıya bir sessionId atıyoruz.
            if (sessionId == null)
            {
                context.HttpContext.Session.SetString("userSessionId", Guid.NewGuid().ToString());
            }

            #region Referans Url Bilgileri Loglanıyor

            var parameters = context.HttpContext.Request.Query;

            if (parameters.ContainsKey("c_name"))
            {
                var referrer = context.HttpContext.Request.GetTypedHeaders().Referer;
                string referrerUrl = referrer != null ? referrer.ToString() : "www.niteliklibilisim.com.tr";
                string campaignName = parameters["c_name"].ToString();
                var ipAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
                var anyIp = _campaignLog.Any(x => x.IpAddress == ipAddress);
                if (!anyIp)
                {
                    CampaignLog newLog = new CampaignLog
                    {
                        RefererUrl = referrerUrl,
                        CampaignName = campaignName,
                        IpAddress = ipAddress
                    };
                    _campaignLog.Create(newLog);
                }
            }
            #endregion

            if (descriptor.ControllerName == "Blog" && descriptor.ActionName == "Detail")
            {
                if (context.ActionArguments["seoUrl"] != null && context.ActionArguments["catSeoUrl"] != null)
                {
                    var ipAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
                    string catSeoUrl = context.ActionArguments["catSeoUrl"].ToString();
                    string seoUrl = context.ActionArguments["seoUrl"].ToString();
                    var anyIp = _blogViewLog.Any(x => x.CatSeoUrl == catSeoUrl && x.SeoUrl == seoUrl && x.IpAddress == ipAddress);
                    if (!anyIp)
                    {
                        BlogViewLog newLog = new BlogViewLog
                        {
                            SeoUrl = context.ActionArguments["seoUrl"].ToString(),
                            CatSeoUrl = context.ActionArguments["catSeoUrl"].ToString(),
                            SessionId = context.HttpContext.Session.GetString("userSessionId"),
                            IpAddress = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                            UserId = context.HttpContext.User.Identity.IsAuthenticated ? context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) : null
                        };
                        _blogViewLog.Create(newLog);
                    }
                }
            }

            #region Create and Insert TransactionLog
            if (currentUserRoleName != "Admin")
            {
                TransactionLog newLog = new TransactionLog
                {
                    ControllerName = descriptor.ControllerName,
                    ActionName = descriptor.ActionName,
                    Parameters = GetParameters(context.ActionArguments),
                    SessionId = context.HttpContext.Session.GetString("userSessionId"),
                    IpAddress = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                    UserId = context.HttpContext.User.Identity.IsAuthenticated ? context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) : null
                };
                _transactionLog.Create(newLog);
            }
            #endregion
        }


        #region Helper Methods
        /// <summary>
        /// ActionArguments içerisindeki parametreleri Liste halinde Log Parameter e map ederek döndürür.
        /// </summary>
        private List<LogParameter> GetParameters(IDictionary<string, object> actionArguments)
        {
            return actionArguments.Where(x => x.Value != null).Select(x => new LogParameter
            {
                ParameterName = x.Key,
                ParameterValue = JsonConvert.SerializeObject(x.Value),
                ParameterType = x.Value.GetType().FullName,
            }).ToList();
        }

        #endregion
    }
}
