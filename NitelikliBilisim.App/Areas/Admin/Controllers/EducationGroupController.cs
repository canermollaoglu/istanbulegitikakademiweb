using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EducationGroupController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationGroupController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult List()
        {
            return View();
        }

        [Route("admin/grup-olustur")]
        public IActionResult Add()
        {
            var model = _unitOfWork.Education.Get(x => x.IsActive, x => x.OrderBy(o => o.CategoryId));
            return View(model);
        }

        public IActionResult GetEducatorsOfEducation(Guid? educationId)
        {

            return Json("");
        }
    }
}