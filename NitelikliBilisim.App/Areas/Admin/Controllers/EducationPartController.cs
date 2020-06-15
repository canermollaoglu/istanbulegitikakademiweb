using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Areas.Admin.VmCreator.EducationParts;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_parts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class EducationPartController : TempSecurityController
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
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationPart");
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
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationUpdatePart");
            if (partId == null)
                return Redirect("/admin/egitimler");

            var part = _unitOfWork.EducationPart.GetById(partId.Value);
            var baseParts = _unitOfWork.EducationPart.Get(x => x.EducationId == part.EducationId && x.BasePartId == null).Select(x => new _EducationPart
            {
                Id = x.Id,
                EducationId = x.EducationId,
                Title = x.Title
            }).ToList();

            var model = new UpdateGetVm
            {
                Id = partId.Value,
                Order = part.Order,
                Title = part.Title,
                EducationId = part.EducationId,
                Duration = part.Duration,
                BasePartId = part.BasePartId,
                BaseParts = baseParts
            };
            return View(model);
        }

        [HttpPost, Route("admin/egitim-parca-guncelle")]
        public IActionResult Update(UpdatePostVm data)
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
            var part = _unitOfWork.EducationPart.GetById(data.PartId);
            part.BasePartId = data.BasePartId;
            part.Order = data.Order.Value;
            part.Title = data.Title;
            part.Duration = data.Duration.Value;
            _unitOfWork.EducationPart.Update(part);
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Eğitim Parça başarıyla güncellenmiştir"
            });
        }
    }
}