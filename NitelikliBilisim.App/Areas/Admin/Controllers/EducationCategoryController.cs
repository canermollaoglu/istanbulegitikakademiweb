using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EducationCategoryController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationCategoryController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Route("admin/kategori-ekle")]
        public IActionResult Add()
        {
            var model = _unitOfWork.EducationCategory.Get(null, q => q.OrderBy(o => o.Name));
            return View(model);
        }

        [HttpPost, Route("admin/kategori-ekle")]
        public JsonResult Add(object o)
        {

            return Json("");
        }

        [Route("admin/kategoriler")]
        public IActionResult List()
        {

            return View();
        }
    }
}