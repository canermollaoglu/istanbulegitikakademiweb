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
    [Area("EducatorArea"), Authorize("Educator")]
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

            return View();
        }
        [Route("egitmen/grup-detayi/{groupId?}")]
        public IActionResult GroupDetails(Guid? groupId)
        {
            if (!groupId.HasValue)
                return Redirect("/egitmen/gruplarim");
            var group = _unitOfWork.EducationGroup.GetById(groupId.Value);
            var educatorId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (group == null || group.EducatorId != educatorId)
                return Redirect("/");

            return View();
        }
    }
}