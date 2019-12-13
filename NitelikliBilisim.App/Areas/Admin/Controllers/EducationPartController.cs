using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Areas.Admin.VmCreator.EducationParts;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_parts;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
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

            _unitOfWork.EducationPart.Delete(partId.Value);
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