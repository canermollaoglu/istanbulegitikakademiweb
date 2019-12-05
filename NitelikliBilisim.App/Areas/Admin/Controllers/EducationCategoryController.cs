using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EducationCategoryController : Controller
    {
        [Route("admin/kategori-ekle")]
        public IActionResult Add()
        {
            return View();
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