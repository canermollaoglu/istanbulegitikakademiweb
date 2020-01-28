using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_groups;
using System;
using System.Linq;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EducationGroupController : TempSecurityController
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationGroupController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Route("admin/gruplar")]
        public IActionResult List()
        {
            var model = _unitOfWork.EducationGroup.GetListVm();
            return View(model);
        }

        [Route("admin/grup-olustur")]
        public IActionResult Add()
        {
            var model = new AddGetVm
            {
                Educations = _unitOfWork.Education.Get(x => x.IsActive, x => x.OrderBy(o => o.CategoryId)),
                Hosts = _unitOfWork.EductionHost.Get(null, x => x.OrderBy(o => o.HostName))
            };
            return View(model);
        }

        [Route("admin/get-assigned-educators-for-group-add/{educationId?}")]
        public IActionResult GetEducatorsOfEducation(Guid? educationId)
        {
            if (!educationId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });

            var model = _unitOfWork.Bridge_EducationEducator.GetAssignedEducators(educationId.Value);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }

        [HttpPost, Route("admin/add-group")]
        public IActionResult Add(AddPostVm data)
        {
            if (!ModelState.IsValid || data.LessonDays == null || data.LessonDays.Count == 0)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = ModelStateUtil.GetErrors(ModelState)
                });

            var isSuccess = _unitOfWork.EducationGroup.Insert(entity: new EducationGroup
            {
                IsGroupOpenForAssignment = true,
                GroupName = data.Name,
                EducationId = data.EducationId.Value,
                EducatorId = data.EducatorId,
                HostId = data.HostId.Value,
                StartDate = data.StartDate.Value,
                Quota = data.Quota.Value
            }, days: data.LessonDays);

            if (isSuccess)
                return Json(new ResponseModel
                {
                    isSuccess = true
                });
            else
                return Json(new ResponseModel
                {
                    isSuccess = false
                });
        }
    }
}