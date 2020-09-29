using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.group_lesson_days;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class GroupLessonDayController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public GroupLessonDayController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("admin/egitim-gunu-guncelle/{lessonDayId}")]
        public IActionResult Update(Guid lessonDayId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationGroupLessonDayDetail");
            var lessonDay = _unitOfWork.GroupLessonDay.GetUpdateGetModel(lessonDayId);
            return View(lessonDay);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(GroupLessonDayUpdatePostVm data)
        {
            var lessonDay = _unitOfWork.GroupLessonDay.GetById(data.Id);
            lessonDay.HasAttendanceRecord = data.HasAttendanceRecord;
            lessonDay.ClassroomId = data.ClassroomId;
            lessonDay.DateOfLesson = data.DateOfLesson;
            lessonDay.EducatorSalary = data.EducatorSalary;
            lessonDay.EducatorId = data.EducatorId;

            _unitOfWork.GroupLessonDay.Update(lessonDay);
            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }

    }
}
