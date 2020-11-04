using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nest;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using System;
using System.Security.Claims;

namespace NitelikliBilisim.App.Filters
{
    public class HandleExceptionAttribute : Attribute, IExceptionFilter
    {
        private readonly IElasticClient _elasticClient;
        public HandleExceptionAttribute(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
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
            CheckExceptionLogIndex();
            var response = _elasticClient.IndexDocument(eInfo);
            Console.Write(response);
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
