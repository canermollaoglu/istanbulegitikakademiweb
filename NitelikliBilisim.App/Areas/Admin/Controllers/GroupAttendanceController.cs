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
        [Route("yoklama-girisi/{groupId?}")]
        public IActionResult ManageGroupAttendance(Guid? groupId)
        {
            if (!groupId.HasValue)
                return Redirect("/");

            return View();
        }
    }
}