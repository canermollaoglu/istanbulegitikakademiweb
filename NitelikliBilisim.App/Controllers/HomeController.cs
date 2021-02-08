using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
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
using NitelikliBilisim.Core.ViewModels.Main.Home;
using NitelikliBilisim.Core.ViewModels.Main.Wizard;
using NitelikliBilisim.Notificator.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly IMemoryCache _memoryCache;
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        public HomeController(IMemoryCache memoryCache, IConfiguration configuration, IEmailSender emailSender, IWebHostEnvironment hostingEnvironment, UnitOfWork unitOfWork, RoleManager<ApplicationRole> roleManager, IElasticClient elasticClient, IHttpContextAccessor httpContextAccessor, IStorageService storageService)
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
            _memoryCache = memoryCache;
        }

        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        public IActionResult Index()
        {
            var model = new HomeIndexModel();
            model.NBUYEducationCategories = _memoryCache.GetOrCreate(CacheKeyUtility.HomeNbuyCategories, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromDays(1);
                return _unitOfWork.EducationCategory.GetNBUYEducationCategories();
            });

            model.EducationComments = _memoryCache.GetOrCreate(CacheKeyUtility.HomeUserComments, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromDays(1);
                return _unitOfWork.EducationComment.GetHighlightComments(5);
            });

            model.EducationSearchTags = _memoryCache.GetOrCreate(CacheKeyUtility.HomeEducationTags, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromDays(1);
                return _unitOfWork.Education.GetEducationSearchTags();
            });
            model.HostCities = EnumHelpers.ToKeyValuePair<HostCity>();

            return View(model);
        }


        //[Route("yakinda")]
        //public IActionResult ComingSoon()
        //{
        //    return View();
        //}


        [Route("gizlilik-sozlesmesi")]
        public IActionResult NonDisclosureAgreement()
        {
            return View();
        }

        [Route("kullanim-kosullari")]
        public IActionResult TermsOfUse()
        {
            return View();
        }

        //public IActionResult Privacy()
        //{
        //    string sessionId = _session.GetString("userSessionId");
        //    string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    ViewData["edl"] = _unitOfWork.Suggestions.GetEducationDetailLogs(userId);
        //    ViewData["TotalRecommendationPoints"] = _unitOfWork.Suggestions.GetEducationSuggestionRate(userId);
        //    return View();
        //}

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
                    Phone = model.Phone,
                    Subject = EnumHelpers.GetDescription(model.ContactFormSubject),
                    Email = model.Email,
                    Content = model.Content,
                    ContactFormType = ContactFormTypes.ContactForm
                });

                string htmlBody = "<b>Konu :</b>" + EnumHelpers.GetDescription(model.ContactFormSubject) + "<br/>";
                htmlBody += "<b>Ad Soyad :</b>" + model.Name + "<br/>";
                htmlBody += "<b>Telefon :</b>" + model.Phone + "<br/>";
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
            catch
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
            catch
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
        public IActionResult UserComments()
        {
            UserCommentsPageGetVm retVal = new();
            retVal.SortingTypes = EnumHelpers.ToKeyValuePair<EducationCommentSortingTypes>();
            retVal.EducationCategories = _unitOfWork.EducationCategory.GetEducationCategoryDictionary();
            retVal.FeaturedComments = _unitOfWork.FeaturedComment.GetFeaturedComments();
            return View(retVal);
        }

        [Route("get-comments")]
        public IActionResult GetFilteredComments(Guid? categoryId, int? sType, int page = 1)
        {
            var comments = _unitOfWork.EducationComment.GetEducationComments(categoryId, sType, page);

            foreach (var comment in comments.Comments)
            {
                comment.AvatarPath = _storageService.BlobUrl + comment.AvatarPath;
                comment.Job = EnumHelpers.GetDescription(comment.JobCode);
            }
            return Json(new ResponseModel
            {
                data = comments
            });
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
                    Phone = model.Phone,
                    RequestNote = model.RequestNote,
                    NumberOfEmployees = model.NumberOfEmployees,
                });
                return Json(new ResponseData
                {
                    Success = true
                });
            }
            catch
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
                    Phone = model.Phone,
                    NameSurname = model.NameSurname,
                    CvUrl = mediaPath
                });
                return Json(new ResponseData
                {
                    Success = true
                });
            }
            catch
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

        [Route("sertifika-dogrula/{certificateCode}")]
        public IActionResult VerifyCertificate(Guid certificateCode)
        {
            var model = _unitOfWork.CustomerCertificate.VerifyCertificate(certificateCode);
            return View(model);
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
                var result = _unitOfWork.SubscriptionBlog.CheckSubscriber(email);
                if (result)
                    return Json(new ResponseData
                    {
                        Success = false,
                        Message = "Bu e-posta adresi ile aktif blog aboneliğiniz bulunmaktadır."
                    });


                _unitOfWork.SubscriptionBlog.Insert(new BlogSubscriber
                {
                    Email = email,
                    Name = name
                });
                return Json(new ResponseData
                {
                    Success = true,
                    Message = "E-bülten'e kaydınız başarıyla sağlanmıştır. Güncel içerikleri tarafınıza ulaştıracağız."
                });
            }
            catch
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

        [Route("wizard-first")]
        [HttpGet]
        public IActionResult WizardGetCategoryDatas()
        {
            var list = _memoryCache.GetOrCreate("wizardfirststep", entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromDays(1);
                return _unitOfWork.Suggestions.GetWizardFirstStepData();
            });

            return Json(new ResponseData
            {
                Success = true,
                Data = list
            });

        }

        [HttpPost]
        [Route("wizard-second")]
        public IActionResult WizardGetSubCategories(List<Guid> relatedCategories)
        {
            try
            {
                var list = _unitOfWork.Suggestions.GetWizardSecondStepData(relatedCategories);

                return Json(new ResponseData
                {
                    Success = true,
                    Data = list
                });
            }
            catch
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
        [Route("wizard-last")]
        public IActionResult WizardSuggestedEducations(List<WizardLastStepPostVm> lastdata)
        {
            try
            {
                var list = _unitOfWork.Suggestions.GetWizardSuggestedEducations(lastdata);
                foreach (var education in list)
                {
                    education.ImageUrl = _storageService.BlobUrl + education.ImageUrl;
                }
                return Json(new ResponseData
                {
                    Success = true,
                    Data = list
                });
            }
            catch
            {
                return Json(new ResponseData
                {
                    Success = false,
                    Message = "Beklenmeyen bir hata ile karşılaşıldı. Lütfen daha sonra tekrar deneyiniz."
                });
            }

        }

    }
}