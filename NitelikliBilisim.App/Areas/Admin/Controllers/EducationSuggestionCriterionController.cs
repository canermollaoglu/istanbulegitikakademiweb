using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities.educations;
using NitelikliBilisim.Core.Enums.educations;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_suggestion_criterion;
using NitelikliBilisim.Support.Enums;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class EducationSuggestionCriterionController : Controller
    {
        private readonly UnitOfWork _unitOfWork;

        public EducationSuggestionCriterionController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("admin/egitim-oneri-kriteri-yonetimi")]
        public IActionResult Manage(Guid? educationId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationSuggestionCriterion");
            if (educationId == null)
                return Redirect("/");

            var education = _unitOfWork.Education.GetById(educationId.Value);
            var model = new EducationSuggestionCriterionManageVm
            {
                EducationId = educationId.Value,
                EducationName = education.Name,
                EducationSuggestionCriterionTypes = EnumSupport.ToKeyValuePair<CriterionType>(),
            };


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(EducationSuggestionCriterionAddVm data)
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
            try
            {
                _unitOfWork.EducationSuggestionCriterion.Insert(new EducationSuggestionCriterion
                {
                    EducationId = data.EducationId,
                    MinValue = data.MinValue,
                    MaxValue = data.MaxValue,
                    CriterionType = (CriterionType)data.CriterionType
                });
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    message = "Öneri Kriteri başarı ile eklenmiştir."
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata " + ex.Message }
                });
            }




        }

        [HttpGet]
        public IActionResult Update(Guid? educationSuggestionCriterionId)
        {
            if (educationSuggestionCriterionId == null)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Öneri kriteri ID değeri okunamadı lütfen tekrar deneyiniz." }
                });
            }

            try
            {
                var data = _unitOfWork.EducationSuggestionCriterion.GetById(educationSuggestionCriterionId.Value);
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = data
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(EducationSuggestionCriterionUpdateVM data)
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
            try
            {
                var suggestionCriterion = _unitOfWork.EducationSuggestionCriterion.GetById(data.Id);
                suggestionCriterion.MinValue = data.MinValue;
                if (data.MaxValue.HasValue && data.MaxValue.Value > 0)
                    suggestionCriterion.MaxValue = data.MaxValue;
                _unitOfWork.EducationSuggestionCriterion.Update(suggestionCriterion);
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    message = "Öneri Kriteri başarı ile güncellenmiştir."
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata " + ex.Message }
                });
            }

        }

        [HttpGet]
        public IActionResult GetList(Guid? educationId)
        {
            if (educationId == null)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Eğitim Öneri Kriterleri yüklenemedi. Lütfen tekrar deneyin." }
                });
            }

            var model = _unitOfWork.EducationSuggestionCriterion.GetByEducationId(educationId.Value);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });

        }

        [HttpGet]
        public IActionResult Delete(Guid? educationSuggestionCriterionId)
        {
            if (educationSuggestionCriterionId == null)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Eğitim Öneri Kriteri silinemedi. Lütfen tekrar deneyin." }
                });
            }
            _unitOfWork.EducationSuggestionCriterion.Delete(educationSuggestionCriterionId.Value);
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Öneri Kriteri başarı ile silinmiştir."
            });
        }
    }
}
