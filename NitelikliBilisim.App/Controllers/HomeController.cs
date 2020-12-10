using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using Nest;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Filters;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.Main.AboutUs;
using NitelikliBilisim.Core.ViewModels.Main.CorporateMembershipApplication;
using NitelikliBilisim.Core.ViewModels.Main.EducationComment;
using NitelikliBilisim.Core.ViewModels.Main.EducatorApplication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Controllers
{

    public class HomeController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly FileUploadManager _fileManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStorageService _storageService;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public HomeController(IWebHostEnvironment hostingEnvironment, UnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager, IElasticClient elasticClient, IHttpContextAccessor httpContextAccessor, IStorageService storageService)
        {
            _hostingEnvironment = hostingEnvironment;
            _fileManager = new FileUploadManager(_hostingEnvironment, "pdf", "doc");
            _httpContextAccessor = httpContextAccessor;
            _roleManager = roleManager;
            CheckRoles().Wait();
            _storageService = storageService;
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
        [Route("gizlilik-sozlesmesi")]
        public IActionResult NonDisclosureAgreement()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            string sessionId = _session.GetString("userSessionId");
            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["edl"] = _unitOfWork.Suggestions.GetEducationDetailLogs(userId);
            ViewData["TotalRecommendationPoints"] = _unitOfWork.Suggestions.GetEducationSuggestionRate(userId);
            return View();
        }

        [Route("hakkimizda")]
        public IActionResult AboutUs()
        {
            AboutUsGetVm model = new();
            model.Hosts = _unitOfWork.EducationHost.EducationHostList();

            return View(model);
        }

        [Route("iletisim")]
        public IActionResult Contact()
        {
            return View();
        }
        [Route("sikca-sorulan-sorular")]
        public IActionResult FrequentlyAskedQuestions()
        {
            return View();
        }

        [Route("kurumsal-uyelik-basvurusu")]
        public IActionResult CorporateMembershipApplication()
        {
            return View();
        }

        [Route("egitmen-basvurusu")]
        public IActionResult EducatorApplication()
        {
            return View();
        }

        [Route("kullanici-yorumlari")]
        public IActionResult UserComments(Guid? c, int? s, int? p)
        {
            UserCommentsPageGetVm retVal = new();

            ViewData["SortingType"] = s.HasValue ? s : ViewData["SortingType"];
            ViewData["Page"] = p.HasValue ? p : ViewData["Page"];
            ViewData["Category"] = c.HasValue ? c : ViewData["Category"];
            //her sayfada 6 yorum.
            retVal.SortingTypes = EnumHelpers.ToKeyValuePair<EducationCommentSortingTypes>();
            retVal.EducationCategories = _unitOfWork.EducationCategory.GetEducationCategoryDictionary();
            retVal.PageDetails = _unitOfWork.EducationComment.GetEducationComments(c,s,p);

            return View(retVal);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("kurumsal-uyelik-basvurusu")]
        public IActionResult CorporateMembershipApplication(CorporateMembershipApplicationAddVm model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _unitOfWork.CorporateMembershipApplication.Insert(new CorporateMembershipApplication
                {
                    CompanyName = model.CompanyName,
                    Address = model.Address,
                    CompanySector = model.CompanySector,
                    NameSurname = model.NameSurname,
                    Department = model.Department,
                    Phone = model.PhoneCode + model.Phone,
                    RequestNote = model.RequestNote,
                    NumberOfEmployees = model.NumberOfEmployees,
                });
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                //Log ex
                return View();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("egitmen-basvurusu")]
        public async Task<IActionResult> EducatorApplication(EducatorApplicationAddVm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var mediaPath = await _storageService.UploadFile(model.Cv.OpenReadStream(), $"{model.NameSurname}.{Path.GetExtension(model.Cv.FileName.ToLower())}", "educator-cv");
                _unitOfWork.EducatorApplication.Insert(new EducatorApplication
                {
                    Email = model.Email,
                    Note = model.Note,
                    Phone = model.PhoneCode + model.Phone,
                    NameSurname = model.NameSurname,
                    CvUrl = mediaPath
                });
                return View();
            }
            catch (Exception ex)
            {
                //Log ex
                return View();
            }
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