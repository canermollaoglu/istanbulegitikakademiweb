using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class EducationCategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}