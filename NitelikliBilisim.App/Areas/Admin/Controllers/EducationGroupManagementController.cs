using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("admin"), Authorize]
    public class EducationGroupManagementController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationGroupManagementController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Route("admin/grup/ayarlar/{groupId?}")]
        public IActionResult Management(Guid? groupId)
        {
            if (!groupId.HasValue)
                return Redirect("admin/gruplar");
            var model = _unitOfWork.EducationGroup.GetById(groupId.Value);
            return View(model);
        }

        [Route("admin/determine-postpone-dates")]
        public IActionResult DeterminePostponeDates(Guid? groupId, DateTime from, DateTime? to)
        {
            if (!groupId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });

            var model = _unitOfWork.GroupLessonDay.DeterminePostponeDates(groupId.Value, from, to);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }
    }
}
