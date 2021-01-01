using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MUsefulMethods;
using Nest;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Filters;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.Main.AboutUs;
using NitelikliBilisim.Core.ViewModels.Main.CorporateMembershipApplication;
using NitelikliBilisim.Core.ViewModels.Main.EducationComment;
using NitelikliBilisim.Core.ViewModels.Main.EducatorApplication;
using NitelikliBilisim.Core.ViewModels.Main.Home;
using NitelikliBilisim.Notificator.Services;
using System;
using System.Collections.Generic;
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
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public HomeController(IConfiguration configuration,IEmailSender emailSender,IWebHostEnvironment hostingEnvironment, UnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager, IElasticClient elasticClient, IHttpContextAccessor httpContextAccessor, IStorageService storageService)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _fileManager = new FileUploadManager(_hostingEnvironment, "pdf", "doc");
            _httpContextAccessor = httpContextAccessor;
            _roleManager = roleManager;
            CheckRoles().Wait();
            _storageService = storageService;
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        public IActionResult Index()
        {
            var model = new HomeIndexModel();
            model.EducationCountByCategory = _unitOfWork.EducationCategory.GetEducationCountForCategories();
            model.EducationComments = _unitOfWork.EducationComment.GetHighlightComments(5);
            model.HostCities = EnumHelpers.ToKeyValuePair<HostCity>();
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
            model.TotalEducatorCount = _unitOfWork.Educator.GetEducatorCount();
            model.TotalEducationHours = _unitOfWork.Education.TotalEducationHours();
            model.Educators = _unitOfWork.Educator.GetEducatorsAboutUsPage();
            return View(model);
        }

        [Route("iletisim")]
        public IActionResult Contact()
        {
            ViewData["ContactFormSubjects"] = EnumHelpers.ToKeyValuePair<ContactFormSubjects>(); 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("iletisim-formu")]
        public IActionResult Contact(ContactFormPostVm model)
        {
            if (!ModelState.IsValid)
                return Json(new ResponseData
                {
                    Success = false
                });

            try
            {
                _unitOfWork.ContactForm.Insert(new ContactForm
                {
                    Name = model.Name,
                    Phone = model.PhoneCode+model.Phone,
                    Subject = EnumHelpers.GetDescription(model.ContactFormSubject),
                    Email = model.Email,
                    Content = model.Content,
                    ContactFormType = ContactFormTypes.ContactForm
                });

                string htmlBody = "<b>Konu :</b>" + EnumHelpers.GetDescription(model.ContactFormSubject)+"<br/>";
                htmlBody += "<b>Ad Soyad :</b>" + model.Name + "<br/>";
                htmlBody += "<b>Telefon :</b>" +model.PhoneCode+ model.Phone + "<br/>";
                htmlBody += "<b>E-Posta :</b>" + model.Email + "<br/>";
                htmlBody += "<b>Mesaj :</b>" + model.Content + "<br/>";
                string[] adminEmails = _configuration.GetSection("SiteGeneralOptions").GetSection("AdminEmails").Value.Split(";");

                _emailSender.SendAsync(new EmailMessage
                {
                    Body = htmlBody,
                    Subject = "Nitelikli Bilişim İletişim Formu",
                    Contacts = adminEmails
                });
               
                return Json(new ResponseData
                {
                    Success = true
                });
            }
            catch (Exception ex)
            {
                //Log ex
                return Json(new ResponseData
                {
                    Success = false
                });
            }

        }

        [Route("sikca-sorulan-sorular")]
        public IActionResult FrequentlyAskedQuestions()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("sss-form")]
        public IActionResult FrequentlyAskedQuestionsForm(FAQContactFormPostVm model)
        {
            if (!ModelState.IsValid)
                return Json(new ResponseData
                {
                    Success = false
                });

            try
            {
                _unitOfWork.ContactForm.Insert(new ContactForm
                {
                    Name = model.Name,
                    Email = model.Email,
                    Content = model.Content,
                    ContactFormType = ContactFormTypes.SSS
                });
                string htmlBody = "<b>Ad Soyad :</b>" + model.Name + "<br/>";
                htmlBody += "<b>E-Posta :</b>" + model.Email + "<br/>";
                htmlBody += "<b>Mesaj :</b>" + model.Content + "<br/>";
                string[] adminEmails = _configuration.GetSection("SiteGeneralOptions").GetSection("AdminEmails").Value.Split(";");

                _emailSender.SendAsync(new EmailMessage
                {
                    Body = htmlBody,
                    Subject = "Nitelikli Bilişim S.S.S. Sayfası İletişim Formu",
                    Contacts = adminEmails
                });

                return Json(new ResponseData
                {
                    Success = true
                });
            }
            catch (Exception ex)
            {
                //Log ex
                return Json(new ResponseData
                {
                    Success = false
                });
            }
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
        public IActionResult UserComments(Guid? c, int? s, int p=1)
        {
            UserCommentsPageGetVm retVal = new();
            ViewData["SortingType"] = s.HasValue ? s : ViewData["SortingType"];
            ViewData["Page"] = p;
            ViewData["Category"] = c.HasValue ? c : ViewData["Category"];
            //her sayfada 6 yorum.
            retVal.SortingTypes = EnumHelpers.ToKeyValuePair<EducationCommentSortingTypes>();
            retVal.EducationCategories = _unitOfWork.EducationCategory.GetEducationCategoryDictionary();
            retVal.PageDetails = _unitOfWork.EducationComment.GetEducationComments(c, s, p);

            //Featured Comments
            retVal.FeaturedComments = _unitOfWork.FeaturedComment.GetFeaturedComments();
            return View(retVal);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("kurumsal-uyelik-basvurusu")]
        public IActionResult CorporateMembershipApplication(CorporateMembershipApplicationAddVm model)
        {
            if (!ModelState.IsValid)
                return Json(new ResponseData
                {
                    Success = false
                });

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
                return Json(new ResponseData
                {
                    Success = true
                });
            }
            catch (Exception ex)
            {
                //Log ex
                return Json(new ResponseData
                {
                    Success = false
                });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("egitmen-basvurusu")]
        public async Task<IActionResult> EducatorApplication(EducatorApplicationAddVm model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new ResponseData
                {
                    Success = false
                });
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
                return Json(new ResponseData
                {
                    Success = true
                });
            }
            catch (Exception ex)
            {
                //Log ex
                return Json(new ResponseData
                {
                    Success = false
                });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("subscribe-blog")]
        public IActionResult SubscribeToBlog(string email, string name)
        {
            if (!ModelState.IsValid)
            {
                return Json(new ResponseData
                {
                    Success = false
                });
            }
            try
            {
               var result= _unitOfWork.SubscriptionBlog.CheckSubscriber(email);
                if (result)
                    return Json(new ResponseData
                    {
                        Success = false,
                        Message ="Zaten blog aboneliğiniz bulunmaktadır."
                    });


                _unitOfWork.SubscriptionBlog.Insert(new BlogSubscriber { 
                Email = email,
                Name = name
                });
                return Json(new ResponseData
                {
                    Success = true,
                    Message= "E-bülten'e kaydınız başarıyla sağlanmıştır. Güncel içerikleri tarafınıza ulaştıracağız."
                });
            }
            catch (Exception ex)
            {
                //Log ex
                return Json(new ResponseData
                {
                    Success = false,
                    Message = "Beklenmeyen bir hata ile karşılaşıldı. Lütfen daha sonra tekrar deneyiniz."
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("subscribe-newsletter")]
        public IActionResult SubscribeToNewsletter(string email)
        {
            if (!ModelState.IsValid)
            {
                return Json(new ResponseData
                {
                    Success = false
                });
            }
            try
            {
                var result = _unitOfWork.SubscriptionNewsletter.CheckSubscriber(email);
                if (result)
                    return Json(new ResponseData
                    {
                        Success = false,
                        Message = "Zaten blog aboneliğiniz bulunmaktadır."
                    });


                _unitOfWork.SubscriptionNewsletter.Insert(new NewsletterSubscriber
                {
                    Email = email
                });
                return Json(new ResponseData
                {
                    Success = true,
                    Message = "Abonelik talebiniz başarı ile alındı."
                });
            }
            catch (Exception ex)
            {
                //Log ex
                return Json(new ResponseData
                {
                    Success = false,
                    Message= "Beklenmeyen bir hata ile karşılaşıldı. Lütfen daha sonra tekrar deneyiniz."
                });
            }
        }
    }
}