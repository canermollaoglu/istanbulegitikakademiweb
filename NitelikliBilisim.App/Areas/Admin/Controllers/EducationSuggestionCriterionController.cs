using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using Newtonsoft.Json;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities.educations;
using NitelikliBilisim.Core.Enums.educations;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_suggestion_criterion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class EducationSuggestionCriterionController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;

        public EducationSuggestionCriterionController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        
        [Route("admin/egitim-oneri-kriteri-listesi")]
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationSuggestionCriterionList");
            return View();
        }

        [Route("admin/egitim-oneri-kriteri-yonetimi")]
        public IActionResult Manage(Guid? educationId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationSuggestionCriterionManage");
            if (educationId == null)
                return Redirect("/");

            var education = _unitOfWork.Education.GetById(educationId.Value);
            var model = new EducationSuggestionCriterionManageVm
            {
                EducationId = educationId.Value,
                EducationName = education.Name,
                EducationSuggestionCriterionTypes = EnumHelpers.ToKeyValuePair<CriterionType>(),
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
                EducationSuggestionCriterion newCriterion = new EducationSuggestionCriterion();
                newCriterion.CriterionType = (CriterionType)data.CriterionType;
                newCriterion.EducationId = data.EducationId;
                if (newCriterion.CriterionType == CriterionType.EducationDay)
                {
                    newCriterion.MinValue = data.MinValue;
                    newCriterion.MaxValue = data.MaxValue;
                }
                else if(newCriterion.CriterionType == CriterionType.WishListEducations || newCriterion.CriterionType == CriterionType.PurchasedEducations)
                {
                    newCriterion.CharValue = JsonConvert.SerializeObject(data.CharValue);
                }
                
                _unitOfWork.EducationSuggestionCriterion.Insert(newCriterion);


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
                var criterions = _unitOfWork.EducationSuggestionCriterion.GetById(educationSuggestionCriterionId.Value);
                EducationSuggestionCriterionUpdateGetVM data = new EducationSuggestionCriterionUpdateGetVM()
                {
                    Id = criterions.Id,
                    CriterionType = criterions.CriterionType,
                    MaxValue = criterions.MaxValue,
                    MinValue = criterions.MinValue,
                    CharValue = criterions.CharValue,
                    AllEducations = null,
                    SelectedEducations = null
                };
                if (data.CriterionType == CriterionType.WishListEducations||data.CriterionType == CriterionType.PurchasedEducations)
                {

                    List<Guid> educationIds = JsonConvert.DeserializeObject<List<Guid>>(data.CharValue);
                    data.AllEducations = _unitOfWork.Education.Get()
                    .Select(x => new KeyValuePair<string, string>(x.Id.ToString(), x.Name))
                    .ToDictionary(x => x.Key, x => x.Value);
                    data.SelectedEducations = data.AllEducations.Where(x => educationIds.Contains(Guid.Parse(x.Key)))
                    .ToDictionary(x => x.Key, x => x.Value);
                }

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
                if (suggestionCriterion.CriterionType == CriterionType.EducationDay)
                {
                    suggestionCriterion.MinValue = data.MinValue;
                    suggestionCriterion.MaxValue = data.MaxValue;
                }
                else if (suggestionCriterion.CriterionType == CriterionType.WishListEducations || suggestionCriterion.CriterionType == CriterionType.PurchasedEducations)
                {
                    suggestionCriterion.CharValue = JsonConvert.SerializeObject(data.CharValue);
                }
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
