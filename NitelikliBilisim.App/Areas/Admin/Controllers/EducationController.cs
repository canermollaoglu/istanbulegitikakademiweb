using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EducationController : Controller
    {
        [Route("admin/egitim-ekle")]
        public IActionResult Add()
        {

            return View();
        }

        [Route("admin/egitimler")]
        public IActionResult List()
        {

            return View();
        }
    }
}