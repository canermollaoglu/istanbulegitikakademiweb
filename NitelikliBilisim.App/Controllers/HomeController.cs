using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nest;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Filters;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Controllers
{
    
    public class HomeController : BaseController
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public HomeController(UnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager, IElasticClient elasticClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _roleManager = roleManager;
            CheckRoles().Wait();
            _unitOfWork = unitOfWork;
        }

        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        public IActionResult Index()
        {
            var model = new HomeIndexModel();
            model.EducationCountByCategory = _unitOfWork.EducationCategory.GetEducationCountForCategories();
            var isLoggedIn = HttpContext.User.Identity.IsAuthenticated;
            if (!isLoggedIn)
                model.SuggestedEducations = _unitOfWork.Suggestions.GetGuestUserSuggestedEducations();
            else
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.SuggestedEducations = _unitOfWork.Suggestions.GetUserSuggestedEducations(userId, 5);
            }

            return View(model);
        }


        [Route("yakinda")]
        public IActionResult ComingSoon()
        {
            return View();
        }

        public IActionResult ErrorTest()
        {
            throw new System.Exception("Test Exception!");
        }

        //[Authorize]
        public IActionResult Privacy()
        {
            string sessionId = _session.GetString("userSessionId");
            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["edl"] = _unitOfWork.Suggestions.GetEducationDetailLogs(userId);
            ViewData["TotalRecommendationPoints"] = _unitOfWork.Suggestions.GetEducationSuggestionRate(userId);
            return View();
        }


        async Task CheckRoles()
        {
            var roles = Enum.GetNames(typeof(IdentityRoleList));
            foreach (var item in roles)
            {
                if (await _roleManager.RoleExistsAsync(item) == false)
                {
                    var result = await _roleManager.CreateAsync(new ApplicationRole()
                    {
                        Name = item
                    });
                }
            }
        }

        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [HttpPost]
        public IActionResult AddWishListItem(Guid? educationId)
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
                    WishlistItem newWishListItem = new WishlistItem();
                    newWishListItem.Id = userId;
                    newWishListItem.Id2 = educationId.Value;
                    _unitOfWork.WishListItem.Insert(newWishListItem);
                    return Json(new ResponseModel
                    {
                        isSuccess = true,
                        message = "Eğitim favori olarak eklenmiştir."
                    });
                }
                else
                {
                    //TODO 
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

        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [HttpPost]
        public IActionResult DeleteWishListItem(Guid? educationId)
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

                _unitOfWork.WishListItem.Delete(userId, educationId.Value);

                return Json(new ResponseModel
                {
                    isSuccess = true,
                    message = "Eğitim favorilerden çıkarılmıştır."
                });


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

    }
}