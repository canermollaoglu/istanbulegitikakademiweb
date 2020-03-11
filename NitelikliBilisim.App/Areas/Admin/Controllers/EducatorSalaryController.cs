using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("admin"), Authorize("admin")]
    public class EducatorSalaryController : Controller
    {
        [Route("admin/ucret-girisi")]
        public IActionResult EnterSalary()
        {
            return View();
        }

        [Route("admin/get-lesson-days-at-date/{date?}")]
        public IActionResult GetLessonDaysAtDate(DateTime? date)
        {
            if (!date.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });

            return Json(new ResponseModel
            {
                isSuccess = true,
                data = null
            });
        }

        [HttpPost, Route("admin/save-salary")]
        public IActionResult SaveSalary()
        {
            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }
    }
}