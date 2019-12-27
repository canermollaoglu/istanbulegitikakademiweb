using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    //[Authorize]
    [Area("admin")]
    public class HomeController : Controller
    {
        [Route("admin/panel")]
        public IActionResult Index()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminHomeIndex");
            return View();
        }
    }
}