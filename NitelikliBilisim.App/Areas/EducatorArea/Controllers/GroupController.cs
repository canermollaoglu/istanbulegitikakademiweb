using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.EducatorArea.Controllers
{
    [Area("EducatorArea"), Authorize(Roles = "Educator")]
    public class GroupController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public GroupController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Route("egitmen/gruplarim")]
        public IActionResult MyGroups()
        {
            var model = _unitOfWork.Educator.GetMyGroupsVm(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View(model);
        }
        [Route("egitmen/grup-detayi/{groupId?}")]
        public IActionResult GroupDetails(Guid? groupId)
        {
            if (!groupId.HasValue)
                return Redirect("/egitmen/gruplarim");
            var group = _unitOfWork.EducationGroup.GetById(groupId.Value);
            if (group == null)
                return Redirect("/");

            var educatorId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _unitOfWork.Educator.GetGroupDetailsVm(groupId.Value, educatorId);
            if (model == null)
                return Redirect("/");
            return View(model);
        }

        [Route("egitmen/yoklama-girisi/{groupId?}/{date?}/{hasAttendanceRecord?}")]
        public IActionResult EnterAttendance(Guid? groupId, DateTime? date, bool? hasAttendanceRecord)
        {
            if (!groupId.HasValue || !date.HasValue || !hasAttendanceRecord.HasValue)
                return Redirect($"/egitmen/gruplarim");
            if (!_unitOfWork.Educator.IsValidEducatorForGroup(groupId.Value, HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)))
                return Redirect($"/egitmen/gruplarim");
            var model = _unitOfWork.GroupAttendance.GetAttendances(groupId.Value, date.Value);
            return View(model);
        }
    }
}