using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.Main.Course;
using System;
using NitelikliBilisim.App.Controllers.Base;

namespace NitelikliBilisim.App.Controllers
{
    //[Authorize]
    public class CourseController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        public CourseController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("kurs-detayi/{courseId}")]
        public IActionResult Details(Guid? courseId)
        {
            var educationDetails = _unitOfWork.Education.GetEducation(courseId.GetValueOrDefault());
            var educators = _unitOfWork.Bridge_EducationEducator.GetAssignedEducators(courseId.GetValueOrDefault());
            var firstAvailableGroup = _unitOfWork.EducationGroup.GetFirstAvailableGroup(courseId.Value);
            GroupVm group = null;
            if (firstAvailableGroup != null)
            {
                group = new GroupVm
                {
                    GroupId = firstAvailableGroup.Id,
                    StartDate = firstAvailableGroup.StartDate,
                    Quota = firstAvailableGroup.Quota
                };
            }
            var model = new CourseDetailsVm
            {
                Details = educationDetails,
                Educators = educators,
                Group = group
            };
            return View(model);
        }
    }
}