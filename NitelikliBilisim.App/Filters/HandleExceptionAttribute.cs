using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.Repositories.MongoDbRepositories;
using NitelikliBilisim.Core.MongoOptions.Entities;
using NitelikliBilisim.Notificator.Services;
using System;
using System.Security.Claims;

namespace NitelikliBilisim.App.Filters
{
    public class HandleExceptionAttribute : Attribute, IExceptionFilter
    {
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ExceptionInfoRepository _eInfo;
        public HandleExceptionAttribute(ExceptionInfoRepository eInfo, IEmailSender emailSender, IConfiguration configuration)
        {
            _emailSender = emailSender;
            _configuration = configuration;
            _eInfo = eInfo;
        }
        public void OnException(ExceptionContext context)
        {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            string mailBody = string.Empty;
            string envName = _configuration.GetSection("SiteGeneralOptions:EnvironmentName").Value;
            ExceptionInfo newLog = new ExceptionInfo
            {
                ControllerName = descriptor.ControllerName,
                ActionName = descriptor.ActionName,
                UserId = context.HttpContext.User.Identity.IsAuthenticated ? context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) : "Anonim",
                Message = context.Exception.Message,
                StackTrace = context.Exception.StackTrace,
                InnerException = context.Exception.InnerException?.Message,
            };
            _eInfo.Create(newLog);

            string adminEmails = _configuration.GetSection("SiteGeneralOptions:AdminEmails").Value;
            mailBody = $"<b>Controller:</b>{newLog.ControllerName}<br>";
            mailBody += $"<b>Action:</b>{newLog.ActionName}<br>";
            mailBody += $"<b>User Id:</b>{newLog.UserId}<br>";
            mailBody += $"<b>Message:</b>{newLog.Message}<br>";
            mailBody += $"<b>StackTrace:</b>{newLog.StackTrace}<br>";
            if (string.IsNullOrEmpty(newLog.InnerException))
                mailBody += $"<b>InnerException:</b>{newLog.InnerException}<br>";

            //Mail Log
            _emailSender.SendAsync(new Core.ComplexTypes.EmailMessage
            {
                Body = mailBody,
                Subject = $"Nitelikli Bilişim {envName} Hata {DateTime.Now:F}",
                Contacts = adminEmails.Split(";")
            });
            
            /*--------*/
            if (!IsAjaxRequest(context.HttpContext.Request))
            {
                context.ExceptionHandled = true;
                context.HttpContext.Response.StatusCode = 500;
                context.Result = new ViewResult()
                {
                    ViewName = "Error"
                };
            }
            else
            {
                context.ExceptionHandled = true;
                context.HttpContext.Response.StatusCode = 500;
                context.Result = new JsonResult(
                    new ResponseModel
                    {
                        isSuccess = false,
                        message = "Beklenmeyen bir hata oluştu! Biz bundan haberdarız ve çalışıyoruz."
                    });
            }
            
            /*---------*/
        }


       
        public static bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}
