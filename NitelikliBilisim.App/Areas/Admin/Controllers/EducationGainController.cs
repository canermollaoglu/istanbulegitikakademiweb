using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Areas.Admin.VmCreator.EducationGains;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_gains;
using System;
using System.Collections.Generic;
using NitelikliBilisim.App.Filters;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class EducationGainController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly EducationGainVmCreator _vmCreator;
        public EducationGainController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _vmCreator = new EducationGainVmCreator(_unitOfWork);
        }
        [Route("admin/egitim-kazanim-yonetimi/{educationId}")]
        public IActionResult Manage(Guid? educationId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationGain");
            if (educationId == null)
                return Redirect("/");

            var model = _vmCreator.CreateManageVm(educationId.Value);

            return View(model);
        }

        [Route("admin/get-education-gains/{educationId}")]
        public IActionResult GetEducationGains(Guid? educationId)
        {
            if (educationId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Eğitimin kazanımlarını getirirken bir hata oluştu" }
                });

            var model = _vmCreator.CreateEducationGainsVm(educationId.Value);

            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }

        [HttpPost, Route("admin/add-education-gain")]
        public IActionResult AddGain(AddGainVm data)
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

            _unitOfWork.EducationGain.Insert(new Core.Entities.EducationGain
            {
                EducationId = data.EducationId,
                Gain = data.Gain
            });

            _unitOfWork.Education.CheckEducationState(data.EducationId);

            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }

        [Route("admin/delete-education-gain/{gainId}")]
        public IActionResult DeleteGain(Guid? gainId)
        {
            if (gainId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Eğitimin kazanımını silerken bir hata oluştu" }
                });

            var educationId = _unitOfWork.EducationGain.GetById(gainId.Value).EducationId;

            _unitOfWork.EducationGain.Delete(gainId.Value);

            _unitOfWork.Education.CheckEducationState(educationId);

            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }

        [Route("admin/egitim-kazanim-guncelle/{gainId}")]
        public IActionResult UpdateGain(Guid? gainId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationUpdateGain");
            if (gainId == null)
                return Redirect("/admin/egitimler");

            var gain = _unitOfWork.EducationGain.GetById(gainId.Value);
            var education = _unitOfWork.Education.GetById(gain.EducationId);
            var model = new UpdateGetVm
            {
                Id = gain.Id,
                EducationId = gain.EducationId,
                EducationName = education.Name,
                Gain = gain.Gain
            };
            return View(model);
        }

        [HttpPost, Route("admin/egitim-kazanim-guncelle")]
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
            var gain = _unitOfWork.EducationGain.GetById(data.GainId);
            gain.EducationId = data.EducationId;
            gain.Gain = data.Gain;
            _unitOfWork.EducationGain.Update(gain);
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Eğitimin Kazanımı başarıyla güncellenmiştir"
            });
        }
    }
}