using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Filters;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.Main.Course;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Controllers
{
    //[Authorize]
    public class CourseController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;
        public CourseController(UnitOfWork unitOfWork,IStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
        }
        [Route("egitimler/{catSeoUrl?}")]
        public IActionResult List(string catSeoUrl)
        {
            CourseListGetVm model = new CourseListGetVm();
            if (string.IsNullOrEmpty(catSeoUrl))
            {
                model.CategoryName = "Tüm Kategoriler";
                model.CategoryShortDescription = "Geleceğin web sitelerini kodlayın";
            }
            else
            {
                var category = _unitOfWork.EducationCategory.GetCategoryBySeoUrl(catSeoUrl);
                if (category != null)
                {
                    model.CategoryId = category.Id;
                    model.CategoryName = category.Name;
                    model.CategoryShortDescription = category.Description;
                }
                else
                {
                    model.CategoryName = "Tüm Kategoriler";
                    model.CategoryShortDescription = "Geleceğin web sitelerini kodlayın";
                }
            }

            model.Categories = _unitOfWork.EducationCategory.GetCoursesPageCategories();
            model.OrderTypes = EnumHelpers.ToKeyValuePair<OrderCriteria>();
            model.EducationHostCities = EnumHelpers.ToKeyValuePair<HostCity>();
            model.TotalEducationCount = _unitOfWork.Education.TotalEducationCount();

            return View(model);
        }


        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [Route("{catSeoUrl}/{seoUrl}")]
        public IActionResult Details(string seoUrl, string searchKey)
        {
            if (string.IsNullOrEmpty(seoUrl))
                return Redirect("/");
            var checkEducation = _unitOfWork.Education.CheckEducationBySeoUrl(seoUrl);
            if (!checkEducation)
                return NotFound();

            var educationDetails = _unitOfWork.Education.GetEducation(seoUrl: seoUrl);
            var educators = _unitOfWork.Bridge_EducationEducator.GetAssignedEducators(educationDetails.Base.Id);
            var isLoggedIn = HttpContext.User.Identity.IsAuthenticated;
            if (isLoggedIn)
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool isCanComment = _unitOfWork.Education.CheckIsCanComment(userId, educationDetails.Base.Id);
                educationDetails.Base.IsCanComment = isCanComment;
                bool status = _unitOfWork.WishListItem.CheckWishListItem(userId, educationDetails.Base.Id);
                educationDetails.Base.IsWishListItem = status;
            }


            var educationCities = _unitOfWork.EducationGroup.GetAvailableCities(educationDetails.Base.Id);

            var model = new CourseDetailsVm
            {
                Details = educationDetails,
                Educators = educators,
                AvaibleEducationCities = educationCities,
                PopularEducations = _unitOfWork.Suggestions.GetGuestUserSuggestedEducations()
            };
            return View(model);
        }

        [Route("usercommentpost")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UserCommentPost(UserCommentPostVm model)
        {
            if (!ModelState.IsValid)
                return Json(new ResponseData
                {
                    Success = false
                });

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _unitOfWork.EducationComment.Insert(new EducationComment
                {
                    ApprovalStatus = Core.Enums.user_details.CommentApprovalStatus.Waiting,
                    Content = model.Content,
                    Points = model.Point,
                    EducationId = model.EducationId,
                    IsEducatorComment = false,
                    CommentatorId = userId
                });
                return Json(new ResponseData
                {
                    Success = true
                });
            }
            catch
            {
                return Json(new ResponseData
                {
                    Success = false
                });
            }
            
        }

        [Route("togglewishlistitem")]
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

        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [HttpPost]
        [Route("get-courses")]
        public async Task<IActionResult> GetCourses(int? hostCity, int page=1,OrderCriteria order = OrderCriteria.Latest)
        {
            
            var model = _unitOfWork.Education.GetCoursesPageEducations(hostCity, page, order);
            
            foreach (var education in model.Educations)
            {
                var folder = Path.GetDirectoryName(education.ImagePath);
                var fileName = Path.GetFileName(education.ImagePath);
                education.ImagePath = await _storageService.DownloadFile(fileName, folder);
            }
            return Json(new ResponseModel
            {
                data = model
            });

        }


    }
}