using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Areas.Admin.VmCreator.Education;
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
        public IActionResult Add(object o)
        {

            return Json("");
        }

        [Route("admin/egitimler")]
        public IActionResult List()
        {

            return View();
        }
    }
}