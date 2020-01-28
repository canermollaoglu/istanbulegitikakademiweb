﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.suggestion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SuggestionController : TempSecurityController
    {
        private readonly UnitOfWork _unitOfWork;
        public SuggestionController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("admin/oneri-kategori-yonetimi")]
        public IActionResult Manage()
        {
            //var categories = _unitOfWork.EducationCategory.GetDeepestCategories(CategoryType.NBUY);
            var categories = _unitOfWork.EducationCategory.Get(x => x.BaseCategoryId == null, x => x.OrderBy(o => o.Name));

            var model = new ManageGetVm
            {
                Categories = categories
            };
            return View(model);
        }
        [Route("admin/get-suggestions")]
        public IActionResult GetSuggestions()
        {
            var model = _unitOfWork.Suggestion.GetSuggestionsVm();

            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }

        [Route("admin/add-suggestion")]
        public IActionResult AddSuggestion(AddPostVm data)
        {
            if (!ModelState.IsValid)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = ModelStateUtil.GetErrors(ModelState)
                });
            var suggestion = _unitOfWork.Suggestion.Get(x => x.CategoryId == data.CategoryId.Value, null).OrderByDescending(x => x.UpdatedDate).First();
            if ((suggestion.RangeMax > data.MaxRange))
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> {$"Kategori'nin En son girilen Maksimum gün değerinden KÜÇÜK bir sayı girmeye çalıştınız" }
                });
            }

            if (suggestion != null) //todo: Bu if her zaman true çıkmaz mı?
            {
                _unitOfWork.Suggestion.Insert(new Suggestion
                {
                    CategoryId = data.CategoryId.Value,
                    RangeMin = Convert.ToByte(suggestion.RangeMax + 1),
                    RangeMax = data.MaxRange.Value
                }, data.SuggestableEducations);

            }
            else
            {
                _unitOfWork.Suggestion.Insert(new Suggestion
                {
                    CategoryId = data.CategoryId.Value,
                    RangeMin = data.MinRange.Value,
                    RangeMax = data.MaxRange.Value
                }, data.SuggestableEducations);
            }


            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }

        [Route("admin/oneri-kategori-guncelle/{suggestionId}")]
        public IActionResult Update (Guid? suggestionId)
        {
            if (suggestionId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Bir hata oluştu" }
                });
            var suggestion = _unitOfWork.Suggestion.GetById(suggestionId.Value);
            var categories = _unitOfWork.EducationCategory.Get(x => x.BaseCategoryId == null, x => x.OrderBy(o => o.Name));
            var data = _unitOfWork.Education.Get(null, x => x.OrderBy(o => o.Name));

            var model = new UpdateGetVm
            {
                CategoryId = suggestion.CategoryId,
                MinRange = suggestion.RangeMin,
                MaxRange = suggestion.RangeMax,
                SuggestableEducations = JsonConvert.DeserializeObject<List<Guid>>(suggestion.SuggestableEducations),
                Categories = categories
            };
            return View(model);
        }

        [Route("admin/delete-suggestion/{suggestionId}")]
        public IActionResult DeleteSuggestion(Guid? suggestionId)
        {
            if (suggestionId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Bir hata oluştu" }
                });

            _unitOfWork.Suggestion.Delete(suggestionId.Value);
            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }

        [Route("admin/get-educations-for-suggestion/{categoryId}")]
        public IActionResult GetEducations(Guid? categoryId)
        {
            if (!categoryId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });

            //var data = _unitOfWork.Education.Get(x => x.CategoryId == categoryId.Value, x => x.OrderBy(o => o.Name));
            var data = _unitOfWork.Education.Get(null, x => x.OrderBy(o => o.Name));

            return Json(new ResponseModel
            {
                isSuccess = true,
                data = data
            });
        }
    }
}