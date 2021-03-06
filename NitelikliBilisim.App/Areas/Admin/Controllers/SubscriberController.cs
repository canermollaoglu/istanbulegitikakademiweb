using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class SubscriberController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        public SubscriberController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [Route("admin/blog-aboneleri")]
        public IActionResult BlogSubscribers()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("BlogSubscribersList");
            return View();
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
