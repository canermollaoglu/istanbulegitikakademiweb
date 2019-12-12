using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Areas.Admin.VmCreator.EducationParts;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;

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
        public IActionResult AddPart()
        {

            return Json("");
        }
    }
}