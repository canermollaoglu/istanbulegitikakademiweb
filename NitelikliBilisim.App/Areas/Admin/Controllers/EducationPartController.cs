using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Areas.Admin.VmCreator.EducationParts;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_parts;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class EducationPartController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly EducationPartVmCreator _vmCreator;
        public EducationPartController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _vmCreator = new EducationPartVmCreator(_unitOfWork);
        }
        [Route("admin/egitim-parca-yonetimi/{educationId}")]
        public IActionResult Manage(Guid? educationId)
        {
            if (educationId == null)
                return Redirect("/");

            var model = _vmCreator.CreateManageVm(educationId.Value);

            return View(model);
        }

        [Route("admin/get-education-parts/{educationId}")]
        public IActionResult GetEducationParts(Guid? educationId)
        {
            if (educationId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Eğitimin parçalarını getirirken bir hata oluştu" }
                });

            var model = _vmCreator.CreateEducationPartsVm(educationId.Value);

            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }

        [HttpPost, Route("admin/add-education-part")]
        public IActionResult AddPart(AddPartVm data)
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

            _unitOfWork.EducationPart.Insert(new Core.Entities.EducationPart
            {
                Title = data.Title,
                EducationId = data.EducationId,
                Order = data.Order.GetValueOrDefault(0),
                Duration = data.Duration.GetValueOrDefault(0)
            });

            _unitOfWork.Education.CheckEducationState(data.EducationId);

            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }

        [Route("admin/delete-education-part/{partId}")]
        public IActionResult DeletePart(Guid? partId)
        {
            if (partId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Eğitimin parçasını silerken bir hata oluştu" }
                });

            var educationId = _unitOfWork.EducationPart.GetById(partId.Value).EducationId;

            _unitOfWork.EducationPart.Delete(partId.Value);

            _unitOfWork.Education.CheckEducationState(educationId);

            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }

        [Route("admin/egitim-parca-guncelle/{partId}")]
        public IActionResult UpdatePart(Guid? partId)
        {
            return View();
        }
    }
}