﻿using Microsoft.AspNetCore.Mvc;
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
                //Yalnızca giriş yapmış ve eğitimi alarak tamamlamış kişiler yorum yapabilir.
                bool isCanComment = _unitOfWork.Education.CheckIsCanComment(userId, courseId.Value);
                educationDetails.Base.IsCanComment = isCanComment;
                //Eğitimin favori eğitimlerde bulunup bulunmaması kontrolü.
                bool status = _unitOfWork.WishListItem.CheckWishListItem(userId, courseId.Value);
                educationDetails.Base.IsWishListItem = status;
            }


            var educationCities = _unitOfWork.EducationGroup.GetAvailableCities(courseId.Value);

            var model = new CourseDetailsVm
            {
                Details = educationDetails,
                Educators = educators,
                AvaibleEducationCities = educationCities,
                PopularEducations = _unitOfWork.Suggestions.GetGuestUserSuggestedEducations()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserCommentPost(UserCommentPostVm model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _unitOfWork.EducationComment.Insert(new EducationComment
                {
                    ApprovalStatus = Core.Enums.user_details.CommentApprovalStatus.Waiting,
                    Content = model.Content,
                    Points = model.Point,
                    EducationId = model.EducationId,
                    IsEducatorComment=false,
                    CommentatorId = userId
                });
            }
            catch (Exception)
            {
                //todo Log
                throw;
            }

            return RedirectToAction("Index", "Home");
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

        [Route("get-available-groups-for-course")]
        public IActionResult GetAvailableGroupsForCourse(Guid? courseId, int city)
        {
            if (!courseId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });

            var model = _unitOfWork.EducationGroup.GetFirstAvailableGroups(courseId.Value, city);
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }
    }
}