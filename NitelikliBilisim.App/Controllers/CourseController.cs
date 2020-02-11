using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.Main.Course;
using System;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.Support.Enums;
using System.Collections.Generic;
using System.Linq;
using NitelikliBilisim.App.Models;

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
            if (!courseId.HasValue)
                return Redirect("/");

            var educationDetails = _unitOfWork.Education.GetEducation(courseId.Value);
            var educators = _unitOfWork.Bridge_EducationEducator.GetAssignedEducators(courseId.Value);

            var model = new CourseDetailsVm
            {
                Details = educationDetails,
                Educators = educators,
            };
            return View(model);
        }

        [Route("get-available-groups-for-course/{courseId?}")]
        public IActionResult GetAvailableGroupsForCourse(Guid? courseId)
        {
            if (!courseId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });

            //var firstAvailableGroups = _unitOfWork.EducationGroup.GetFirstAvailableGroups(courseId.Value);

            //var model = firstAvailableGroups.Select(x => new GroupVm
            //{
            //    GroupId = x.Id,
            //    StartDate = x.StartDate,
            //    Quota = x.Quota,
            //    Host = new HostVm
            //    {
            //        HostId = x.Host.Id,
            //        Address = x.Host.Address,
            //        City = EnumSupport.GetDescription(x.Host.City),
            //        HostName = x.Host.HostName,
            //        Latitude = x.Host.Latitude,
            //        Longitude = x.Host.Longitude
            //    }
            //}).ToList();
            var model = _unitOfWork.EducationGroup.GetFirstAvailableGroups(courseId.Value);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }
    }
}