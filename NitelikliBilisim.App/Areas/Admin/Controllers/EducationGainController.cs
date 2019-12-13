using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Areas.Admin.VmCreator.EducationGains;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_gains;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EducationGainController : Controller
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

            _unitOfWork.EducationGain.Delete(gainId.Value);
            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }

        [Route("admin/egitim-kazanim-guncelle/{gainId}")]
        public IActionResult UpdateGain(Guid? gainId)
        {
            return View();
        }
    }
}