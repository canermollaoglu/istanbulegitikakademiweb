using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Areas.Admin.VmCreator.EducationParts;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_parts;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    //[Authorize]
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

        [Route("admin/get-base-parts/{educationId}")]
        public IActionResult GetEducationBaseParts(Guid? educationId)
        {
            if (educationId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Eğitimin parçalarını getirirken bir hata oluştu" }
                });

            var model = _vmCreator.CreateBaseParts(educationId.Value);

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

            if (data.BasePartId.HasValue && !_unitOfWork.EducationPart.IsBasePart(data.BasePartId.Value))
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Seçilmiş olan parça üst başlık olamaz. Bu parça zaten başka bir üst başlığa ait." }
                });

            _unitOfWork.EducationPart.Insert(new EducationPart
            {
                Title = data.Title,
                EducationId = data.EducationId,
                Order = data.Order.GetValueOrDefault(0),
                Duration = data.Duration.GetValueOrDefault(0),
                BasePartId = data.BasePartId
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

            if (_unitOfWork.EducationPart.HasSubParts(partId.Value))
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Silinmek istenilen parçaya ait parçalar vardır. Öncelikle alt başlıklıkları siliniz." }
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