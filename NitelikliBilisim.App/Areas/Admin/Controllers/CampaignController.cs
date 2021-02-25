using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities.campaign;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class CampaignController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        public CampaignController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Route("/admin/kampanya-listesi")]
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminCampaignList");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Campaign data)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelStateUtil.GetErrors(ModelState);
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = errors
                });
            }
            _unitOfWork.Campaign.Insert(data);
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Kampanya başarı ile eklenmiştir."
            });
        }

        public IActionResult Delete(Guid? campaignId)
        {
            if (!campaignId.HasValue)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata: Sayfayı yenileyerek tekrar deneyiniz." }
                });
            }
            _unitOfWork.Campaign.Delete(campaignId.Value);
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Kategori başarıyla silinmiştir."
            });

        }

        [HttpGet]
        [Route("/admin/kampanya-guncelle")]
        public IActionResult Update(Guid? campaignId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminCampaignUpdate");
            if (campaignId == null)
                return Redirect("/admin/kampanya-listesi");
            var category = _unitOfWork.Campaign.GetById(campaignId.Value);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin/kampanya-guncelle")]
        public IActionResult Update(Campaign data)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelStateUtil.GetErrors(ModelState);
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = errors
                });
            }
            _unitOfWork.Campaign.Update(data);
            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }
    }
}
