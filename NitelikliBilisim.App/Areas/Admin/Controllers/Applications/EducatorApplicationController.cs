using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Services.Abstracts;
using System;
using System.Collections.Generic;
using System.IO;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.Applications
{
    public class EducatorApplicationController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IStorageService _storage;
        public EducatorApplicationController(UnitOfWork unitOfWork,IStorageService storage)
        {
            _unitOfWork = unitOfWork;
            _storage = storage;
        }
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducatorApplicationList");
            return View();
        }
        [Route("admin/egitmen-cv-goruntule")]
        public IActionResult Cv(Guid eId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducatorApplicationCv");
            string fullPath = string.Empty;
            var educatorApplication = _unitOfWork.EducatorApplication.GetById(eId);
            try
            {
                fullPath = _storage.DownloadFile(Path.GetFileName(educatorApplication.CvUrl), "educator-application-cv").Result;
            }
            catch
            {
            }
            ViewData["CvUrl"] = fullPath;
            return View();
        }

        /// <summary>
        /// Kayıt görüldü bilgisini true olarak günceller.
        /// </summary>
        /// <param name="eAId">educatorApplicationId</param>
        /// <returns></returns>
        public IActionResult SetViewedStatus(Guid eAId)
        {
            try
            {
                var educatorApplication = _unitOfWork.EducatorApplication.GetById(eAId);
                educatorApplication.IsViewed = true;
                _unitOfWork.EducatorApplication.Update(educatorApplication);
                return Json(new ResponseModel
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { $"Hata: {ex.Message}" }
                });
            }
        }
    }
}
