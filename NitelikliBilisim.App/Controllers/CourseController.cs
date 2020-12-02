using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.Main.Course;
using System;
using NitelikliBilisim.App.Filters;
using MUsefulMethods;
using NitelikliBilisim.Core.Enums;
using System.Collections.Generic;
using NitelikliBilisim.Core.Entities;
using System.Security.Claims;

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

        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [Route("kurs-detayi/{courseId}")]
        public IActionResult Details(Guid? courseId, string searchKey)
        {
            if (!courseId.HasValue)
                return Redirect("/");

            var educationDetails = _unitOfWork.Education.GetEducation(courseId.Value);
            var educators = _unitOfWork.Bridge_EducationEducator.GetAssignedEducators(courseId.Value);
            var isLoggedIn = HttpContext.User.Identity.IsAuthenticated;
            if (isLoggedIn)
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool status = _unitOfWork.WishListItem.CheckWishListItem(userId, courseId.Value);
                educationDetails.Base.IsWishListItem = status;
            }


            var educationCities = _unitOfWork.EducationGroup.GetAvailableCities(courseId.Value);

            var model = new CourseDetailsVm
            {
                Details = educationDetails,
                Educators = educators,
                AvaibleEducationCities = educationCities
            };
            return View(model);
        }

        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [HttpPost]
        public IActionResult ToggleWishListItem(Guid? educationId)
        {
            if (!educationId.HasValue)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Lütfen sayfayı yenileyerek tekrar deneyiniz." }
                });
            }

            try
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null)
                {
                    WishlistItem wishListItem = new WishlistItem();
                    wishListItem.Id = userId;
                    wishListItem.Id2 = educationId.Value;
                   var retVal = _unitOfWork.WishListItem.ToggleWishListItem(wishListItem);
                    return Json(new ResponseModel
                    {
                        isSuccess = true,
                        data = retVal
                    });
                }
                else
                {
                    return Json(new ResponseModel
                    {
                        isSuccess = true,
                        message = "Eğitim favori olarak eklenmiştir. Giriş yaparak işlemi tamamlayabilirsiniz. "
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Bir sorunla karşılaşıldı." }
                });
            }

        }

        [Route("get-available-groups-for-course/{courseId?}")]
        public IActionResult GetAvailableGroupsForCourse(Guid? courseId)
        {
            if (!courseId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });

            var model = _unitOfWork.EducationGroup.GetFirstAvailableGroups(courseId.Value);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }
    }
}