using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_group_attendances;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GroupAttendanceController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public GroupAttendanceController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Route("admin/yoklama-girisi/{groupId?}/{date?}/{hasAttendanceRecord?}")]
        [Authorize(Roles = "Admin")]
        public IActionResult EnterAttendance(Guid? groupId, DateTime? date, bool? hasAttendanceRecord)
        {
            if (!groupId.HasValue || !date.HasValue || !hasAttendanceRecord.HasValue)
                return Redirect($"/admin/grup/ayarlar/{groupId.Value}");
            var model = _unitOfWork.GroupAttendance.GetAttendances(groupId.Value, date.Value);
            return View(model);
        }

        [HttpPost, Route("yoklamalari-kaydet"), Authorize(Roles = "Admin,Educator")]
        public IActionResult SaveAttendances(AttendanceData data)
        {
            if (User.IsInRole("Educator") && data.Date.AddDays(7).Date > DateTime.Now.Date)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    message = "Yoklama girişinin zamanı geçmiştir"
                });
            _unitOfWork.GroupAttendance.SaveAttendances(data);
            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }
    }
}