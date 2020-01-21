using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EducationGroupController : Controller
    {
        public IActionResult List()
        {
            return View();
        }

        [Route("admin/grup-olustur")]
        public IActionResult Add()
        {
            return View();
        }
    }
}