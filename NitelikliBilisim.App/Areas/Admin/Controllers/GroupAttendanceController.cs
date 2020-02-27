using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class GroupAttendanceController : Controller
    {
        [Route("admin/yoklama-girisi/{groupId?}/{date?}/{hasAttendanceRecord?}")]
        public IActionResult EnterAttendance(Guid? groupId, DateTime? date, bool? hasAttendanceRecord)
        {
            if (!groupId.HasValue || !date.HasValue || !hasAttendanceRecord.HasValue)
                return Redirect($"/admin/grup/ayarlar/{groupId.Value}");

            return View();
        }
    }
}