using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        public CourseController(UserManager<ApplicationUser> userManager,UnitOfWork unitOfWork,IStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _userManager = userManager;
        }
        [Route("egitimler/{catSeoUrl?}")]
        public IActionResult List(string catSeoUrl)
        {
            CourseListGetVm model = new CourseListGetVm();
            if (string.IsNullOrEmpty(catSeoUrl))
            {
                model.CategoryName = "Tüm Teknoloji Eğitimleri";
                model.CategoryShortDescription = "Tüm Teknoloji Eğitimleri";
                model.CategoryIconColor = "#459af0";
                model.CategoryIconUrl = "icon-search";
            }
            else
            {
                var category = _unitOfWork.EducationCategory.GetCategoryBySeoUrl(catSeoUrl);
                if (category != null)
                {
                    model.CategoryId = category.Id;
                    model.CategoryName = category.Name;
                    model.CategoryShortDescription = category.Description;
                    model.CategoryIconColor = category.IconColor;
                    model.CategoryIconUrl = category.IconUrl;
                }
                else
                {
                    model.CategoryName = "Tüm Teknoloji Eğitimleri";
                    model.CategoryShortDescription = "Tüm Teknoloji Eğitimleri";
                    model.CategoryIconColor = "#459af0";
                    model.CategoryIconUrl = "icon-search";
                }
            }

            model.Categories = _unitOfWork.EducationCategory.GetCoursesPageCategories();
            model.OrderTypes = EnumHelpers.ToKeyValuePair<OrderCriteria>();
            model.EducationHostCities = EnumHelpers.ToKeyValuePair<HostCity>();
            model.TotalEducationCount = _unitOfWork.Education.TotalEducationCount();

            return View(model);
        }

        [Route("arama-sonuclari")]
        public IActionResult SearchResults(string s,HostCity h)
        {
            SearchResultsVm model = new SearchResultsVm();

            model.Categories = _unitOfWork.EducationCategory.GetCoursesPageCategories();
            model.OrderTypes = EnumHelpers.ToKeyValuePair<OrderCriteria>();
            model.EducationHostCities = EnumHelpers.ToKeyValuePair<HostCity>();
            model.TotalEducationCount = _unitOfWork.Education.TotalEducationCount();
            model.SearchKey = s;
            model.HostCity = (int)h;
            return View(model);
        }

        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [Route("{catSeoUrl}/{seoUrl}")]
        public async Task<IActionResult> Details(string seoUrl, string searchKey)
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
                var user = await _userManager.FindByIdAsync(userId);
                var student = _unitOfWork.Customer.GetById(userId);
                educationDetails.CurrentUserName = $"{user.Name} {user.Surname}";
                educationDetails.CurrentUserJob = student == null ? "Eğitmen" : EnumHelpers.GetDescription(student.Job);
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
        public IActionResult GetCourses(Guid? categoryId,int? hostCity, string searchKey, int page=1,OrderCriteria order = OrderCriteria.Latest)
        {
            
            var model = _unitOfWork.Education.GetCoursesPageEducations(categoryId,hostCity, page, order,searchKey);
            
            foreach (var education in model.Educations)
            {
                education.ImagePath = _storageService.BlobUrl + education.ImagePath;
            }
            return Json(new ResponseModel
            {
                data = model
            });

        }


        [Route("get-course-comments")]
        public IActionResult GetCourseComments(Guid educationId, int page)
        {
            var model = _unitOfWork.EducationComment.GetPagedComments(educationId, page);

            foreach (var comment in model.Comments)
            {
                comment.UserAvatarPath=  _storageService.BlobUrl+comment.UserAvatarPath;
            }
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }

    }
}