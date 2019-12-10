using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Areas.Admin.Models.Education;
using NitelikliBilisim.App.Areas.Admin.VmCreator.Education;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EducationController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly EducationVmCreator _vmCreator;
        public EducationController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _vmCreator = new EducationVmCreator(_unitOfWork);
        }
        [Route("admin/egitim-ekle")]
        public IActionResult Add()
        {
            var model = _vmCreator.CreateAddGetVm();
            return View(model);
        }

        [HttpPost, Route("admin/egitim-ekle")]
        public IActionResult Add(AddPostVm data)
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

            _unitOfWork.Education.Insert(new Core.Entities.Education
            {
                Name = data.Name,
                Level = (Core.Enums.EducationLevel)data.EducationLevel.Value,
                NewPrice = data.Price.Value,
                Days = data.Days.Value,
                HoursPerDay = data.HoursPerDay.Value
            }, data.CategoryIds);

            return Json(new ResponseModel
            {
                isSuccess =  true,
                message = "Eğitim başarıyla eklenmiştir"
            });
        }

        [Route("admin/egitimler")]
        public IActionResult List()
        {

            return View();
        }
    }
}