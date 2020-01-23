using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.Main.Course;
using System;

namespace NitelikliBilisim.App.Controllers
{
    //[Authorize]
    public class CourseController : Controller
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
            var lastAvailableGroup = _unitOfWork.EducationGroup.GetLastAvailableGroup(courseId.GetValueOrDefault());
            DateTime? startDate = null;
            if (lastAvailableGroup != null)
                startDate = lastAvailableGroup.StartDate;
            var model = new CourseDetailsVm
            {
                Details = educationDetails,
                Educators = educators,
                StartDate = startDate
            };
            return View(model);
        }
    }
}