using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class StudentController : Controller
    {
        [Route("admin/ogrenci-yonetimi")]
        public IActionResult Index()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminStudentList");
            return View();
        }

        [Route("admin/ogrenci-hareketleri")]
        public IActionResult StudentLogs(string studentId)
        {

            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminStudentLogList");
            ViewData["studentId"] = studentId;

            return View();
        }

    }
}
