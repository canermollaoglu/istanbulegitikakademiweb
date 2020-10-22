using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.Applications
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class EducatorApplicationController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public EducatorApplicationController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducatorApplicationList");
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
