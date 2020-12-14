using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Nest;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Core.ESOptions.ESEntities;
using NitelikliBilisim.Notificator.Services;
using System;
using System.Security.Claims;

namespace NitelikliBilisim.App.Filters
{
    public class HandleExceptionAttribute : Attribute, IExceptionFilter
    {
        private readonly IElasticClient _elasticClient;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        public HandleExceptionAttribute(IElasticClient elasticClient, IEmailSender emailSender, IConfiguration configuration)
        {
            _elasticClient = elasticClient;
            _emailSender = emailSender;
            _configuration = configuration;
        }
        public void OnException(ExceptionContext context)
        {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            string mailBody = string.Empty;
            var server = string.Empty;
#if  DEBUG
            server = "Test Ortamı";
#endif
            var eInfo = new ExceptionInfo
            {
                ControllerName = descriptor.ControllerName,
                ActionName = descriptor.ActionName,
                UserId = context.HttpContext.User.Identity.IsAuthenticated ? context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) : "Anonim",
                Message = context.Exception.Message,
                StackTrace = context.Exception.StackTrace,
                InnerException = context.Exception.InnerException?.Message,
            };
            string adminEmails = _configuration.GetSection("SiteGeneralOptions:AdminEmails").Value;
            mailBody = $"<b>Controller:</b>{eInfo.ControllerName}<br>";
            mailBody += $"<b>Action:</b>{eInfo.ActionName}<br>";
            mailBody += $"<b>User Id:</b>{eInfo.UserId}<br>";
            mailBody += $"<b>Message:</b>{eInfo.Message}<br>";
            mailBody += $"<b>StackTrace:</b>{eInfo.StackTrace}<br>";
            if (string.IsNullOrEmpty(eInfo.InnerException))
                mailBody += $"<b>InnerException:</b>{eInfo.InnerException}<br>";


            //Mail Log
            _emailSender.SendAsync(new Core.ComplexTypes.EmailMessage
            {
                Body = mailBody,
                Subject = $"Nitelikli Bilişim {server} Hata {DateTime.Now:F}",
                Contacts = adminEmails.Split(";")
            });


            //EsInsert
            CheckExceptionLogIndex();
            var response = _elasticClient.IndexDocument(eInfo);
        }


        /// <summary>
        /// Indıcesin varlığını kontrol edip yoksa oluşturuyor.
        /// </summary>
        private void CheckExceptionLogIndex()
        {
            var response = _elasticClient.Indices.Exists(ElasticSearchIndexNameUtility.ExceptionLogIndex);
            if (!response.Exists)
            {
                _elasticClient.Indices.Create(ElasticSearchIndexNameUtility.ExceptionLogIndex, index =>
                   index.Map<ExceptionInfo>(x => x.AutoMap()));
            }
        }

        /// <summary>
        /// Determines whether the specified HTTP request is an AJAX request.
        /// </summary>
        /// 
        /// <returns>
        /// true if the specified HTTP request is an AJAX request; otherwise, false.
        /// </returns>
        /// <param name="request">The HTTP request.</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> parameter is null (Nothing in Visual Basic).</exception>
        public static bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}
