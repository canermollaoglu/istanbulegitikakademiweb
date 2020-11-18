using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nest;
using Newtonsoft.Json;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Notificator.Services;
using System;
using System.Security.Claims;

namespace NitelikliBilisim.App.Filters
{
    public class HandleExceptionAttribute : Attribute, IExceptionFilter
    {
        private readonly IElasticClient _elasticClient;
        private readonly IEmailSender _emailSender;
        public HandleExceptionAttribute(IElasticClient elasticClient,IEmailSender emailSender)
        {
            _elasticClient = elasticClient;
            _emailSender = emailSender;
        }
        public void OnException(ExceptionContext context)
        {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            
            var eInfo = new ExceptionInfo
            {
                ControllerName = descriptor.ControllerName,
                ActionName = descriptor.ActionName,
                UserId = context.HttpContext.User.Identity.IsAuthenticated ? context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) : "Anonim",
                Message = context.Exception.Message,
                StackTrace = context.Exception.StackTrace,
                InnerException = context.Exception.InnerException?.Message,
            };
             _emailSender.SendAsync(new Core.ComplexTypes.EmailMessage
            {
                Body = JsonConvert.SerializeObject(eInfo),
                Subject = "Nitelikli Bilişim Hata",
                Contacts = new string[] { "asim.ulusoy@wissenakademie.com" }
            });

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
    }
}
