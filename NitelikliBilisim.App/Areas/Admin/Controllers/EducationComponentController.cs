using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using MUsefulMethods;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_component;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class EducationComponentController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;

        public EducationComponentController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            var model = new EducationComponentAddGetVm
            {
                ComponentTypes = EnumHelpers.ToKeyValuePair<EducationComponentType>(),
                SuggestionTypes = EnumHelpers.ToKeyValuePair<EducationComponentSuggestionType>(),
                Educations = _unitOfWork.Education.Get(x => x.IsActive).ToList()
            };

            return View(model);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(EducationComponentAddPostVm data)
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

            _unitOfWork.EducationComponentItem.Insert(new EducationComponentItem()
            {
                EducationId = data.EducationId,
                ComponentType = data.ComponentType,
                SuggestionType = data.SuggestionType,
                Order = data.Order
            });


            return Json(new ResponseModel
            {
                isSuccess = true
            });


        }

        [Route("admin/delete-education-component-item")]
        public IActionResult Delete(Guid? componentItemId)
        {
            if (componentItemId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Eğitimin kazanımını silerken bir hata oluştu" }
                });

           
            _unitOfWork.EducationComponentItem.Delete(componentItemId.Value);


            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }


    }
}
