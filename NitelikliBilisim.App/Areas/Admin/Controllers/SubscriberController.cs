using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.areas.admin.subscriber;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class SubscriberController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMessageService _messageService;
        public SubscriberController(UnitOfWork unitOfWork,IMessageService messageService)
        {
            _unitOfWork = unitOfWork;
            _messageService = messageService;
        }


        [Route("admin/blog-aboneleri")]
        public IActionResult BlogSubscribers()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("BlogSubscribersList");
            return View();
        }
        [Route("admin/yeni-gonderi-yayinla")]
        public IActionResult CreateNewPost(SubscriptionBroadcastType type)
        {

            var templates = type == SubscriptionBroadcastType.NewsletterBroadcast ?
                _unitOfWork.EmailTemplate.Get(x => x.Type == EmailTemplateType.NewsletterSubscribersPostTemplate)
                            .ToDictionary(x => x.Id, x => x.Name)
                            : _unitOfWork.EmailTemplate.Get(x => x.Type == EmailTemplateType.BlogSubscribersPostTemplate)
                            .ToDictionary(x => x.Id, x => x.Name);
            ViewData["type"] = type;

            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("SubscribersCreateNewPost");
            return View(templates);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/yeni-gonderi-yayinla")]
        public IActionResult SendPost(SubscriberPostVm data)
        {
            var subscribers = new List<string>();
            if (data.Type == SubscriptionBroadcastType.BlogBroadcast)
            {
                subscribers.AddRange(_unitOfWork.SubscriptionBlog.Get(x => !x.IsCanceled).Select(x => x.Email));
            }
            else if(data.Type == SubscriptionBroadcastType.NewsletterBroadcast)
            {
                subscribers.AddRange(_unitOfWork.SubscriptionNewsletter.Get(x => !x.IsCanceled).Select(x => x.Email));
            }

            var message = new EmailMessage
            {
                Subject = data.Title,
                Body = data.Content,
                Contacts = subscribers.ToArray()
            };
            _messageService.SendAsync(JsonConvert.SerializeObject(message));

            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Gönderi başarı ile yayınlanmıştır."
            });

        }


        [Route("admin/email-tema-icerik")]
        public IActionResult GetEmailTemplateContent(Guid templateId)
        {

            var template = _unitOfWork.EmailTemplate.GetById(templateId).Content;
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = template
            });

        }

        [Route("admin/blog-abonesi-sil")]
        public IActionResult DeleteBlogSubscriber(Guid? subscriberId)
        {
            if (subscriberId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    message = "Silinecek veri bulunamadı"
                });
            var entity = _unitOfWork.SubscriptionBlog.GetById(subscriberId.Value);
            if (entity != null)
            {
                _unitOfWork.SubscriptionBlog.Delete(entity);

            }

            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Silme işlemi başarılı"
            });
        }


        [Route("admin/haber-bulteni-aboneleri")]
        public IActionResult NewsletterSubscribers()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("NewsLetterSubscribersList");
            return View();
        }

        [Route("admin/haber-bulteni-abonesi-sil")]
        public IActionResult DeleteNewsletterSubscriber(Guid? subscriberId)
        {
            if (subscriberId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    message = "Silinecek veri bulunamadı"
                });
            _unitOfWork.SubscriptionNewsletter.Delete(subscriberId.Value);
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Silme işlemi başarılı"
            });
        }
    }
}
