using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class GroupAttendanceController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public GroupAttendanceController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Route("admin/yoklama-girisi/{groupId?}/{date?}/{hasAttendanceRecord?}")]
        public IActionResult EnterAttendance(Guid? groupId, DateTime? date, bool? hasAttendanceRecord)
        {
            if (!groupId.HasValue || !date.HasValue || !hasAttendanceRecord.HasValue)
                return Redirect($"/admin/grup/ayarlar/{groupId.Value}");
            var model = _unitOfWork.GroupAttendance.GetAttendances(groupId.Value, date.Value);
            return View(model);
        }

        public IActionResult SaveAttendances(AttendanceData data)
        {

            return Json("");
        }
    }

    public class AttendanceData
    {
        public Guid GroupId { get; set; }
        public DateTime Date { get; set; }
        public List<StudentRecord> StudentRecords { get; set; }
    }
    public class StudentRecord
    {
        public string CustomerId { get; set; }
        public bool IsAttended { get; set; }
        public string Reason { get; set; }
    }
}