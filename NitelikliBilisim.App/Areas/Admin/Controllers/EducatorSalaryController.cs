using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.areas.admin.educator_salary;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class EducatorSalaryController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        public EducatorSalaryController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
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
            var model = _unitOfWork.Salary.GetSalaries(date.Value);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }

        [HttpPost, Route("admin/save-salary")]
        public IActionResult SaveSalary(SaveSalaryPostData data)
        {
            _unitOfWork.Salary.SaveSalaries(data);
            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }
    }
}