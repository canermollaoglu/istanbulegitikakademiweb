using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.Main.Course;

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
            var educationDetails = _unitOfWork.Education.GetEducation(courseId.Value);
            var educators = _unitOfWork.Bridge_EducationEducator.GetAssignedEducators(courseId.Value);

            var model = new CourseDetailsVm
            {
                Details = educationDetails,
                Educators = educators
            };
            return View(model);
        }
    }
}