
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Filters;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.Main.Profile;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Controllers
{
    [Authorize]
    public class UserProfileController : BaseController
    {
        private readonly UserUnitOfWork _userUnitOfWork;
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStorageService _storageService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly FileUploadManager _fileManager;
        public UserProfileController(UnitOfWork unitOfWork, UserUnitOfWork userUnitOfWork, UserManager<ApplicationUser> userManager, IStorageService storageService, IWebHostEnvironment hostingEnvironment)
        {
            _userUnitOfWork = userUnitOfWork;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _hostingEnvironment = hostingEnvironment;
            _fileManager = new FileUploadManager(_hostingEnvironment, "jpg", "jpeg");
        }
        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [Route("hesap/panelim")]
        public IActionResult Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var model = _userUnitOfWork.User.GetCustomerInfo(userId);
            var model = _userUnitOfWork.User.GetPanelInfo(userId);
            return View(model);
        }
        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [Route("gruplarim/{ticketId?}")]
        public IActionResult MyGroup(Guid? ticketId)
        {
            if (!ticketId.HasValue)
                return Redirect($"/profil/{User.FindFirstValue(ClaimTypes.NameIdentifier)}");
            var model = _userUnitOfWork.Group.GetMyGroupVm(ticketId.Value);
            if (model == null)
                return Redirect($"/profil/{User.FindFirstValue(ClaimTypes.NameIdentifier)}");
            return View(model);
        }
        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [Route("faturalarim")]
        public IActionResult MyInvoices()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _userUnitOfWork.User.GetUserInvoices(userId);
            if (model == null)
                return Redirect($"/profil/{userId}");
            return View(model);
        }

        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        public IActionResult Cancellation(Guid? ticketId)
        {
            if (!ticketId.HasValue)
                return Redirect($"/profil/{User.FindFirstValue(ClaimTypes.NameIdentifier)}");

            return View();
        }
        [Route("hesap/menu")]
        public IActionResult MyMenu()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(_userUnitOfWork.User.GetMyAccountSidebarInfo(userId, null));
        }

        [Route("hesap/kurslarim")]
        public IActionResult MyCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _userUnitOfWork.User.GetPurschasedEducationsByUserIdMyCoursesPage(userId);
            return View(model);
        }

        [Route("hesap/kurs-detay/{groupId}")]
        public IActionResult MyCourseDetail(Guid groupId)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool check = _userUnitOfWork.User.CheckPurschasedEducation(userId, groupId);
            if (!check)
                return RedirectToAction("MyCourses");

            var model = _userUnitOfWork.User.GetPurschasedEducationDetail(userId, groupId);
            if (model == null)
                return RedirectToAction("MyCourses");
            return View(model);
        }
        [Route("hesap/yorumlarim")]
        public IActionResult MyComments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _userUnitOfWork.User.GetCustomerComments(userId);
            return View(model);
        }
        [Route("hesap/faturalarim")]
        public IActionResult MyInvoiceList()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _userUnitOfWork.User.GetCustomerInvoices(userId);
            return View(model);
        }
        [Route("hesap/fatura-detay")]
        public IActionResult InvoiceDetails(Guid invoiceId)
        {
            var model = _userUnitOfWork.User.GetCustomerInvoiceDetails(invoiceId);
            return View(model);
        }
        [Route("hesap/indirim-kuponlarim")]
        public IActionResult MyCoupons()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _userUnitOfWork.User.GetCustomerPromotions(userId);
            return View(model);
        }
        [Route("hesap/favori-kurslarim")]
        public IActionResult MyFavoriteCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _userUnitOfWork.User.GetUserFavoriteEducationsByUserId(userId);
            return View(model);
        }
        [Route("hesap/ayarlar")]
        public IActionResult AccountSettings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _userUnitOfWork.User.GetAccoutSettingsPageData(userId);
            return View(model);
        }

        [Route("sana-ozel")]
        public IActionResult ForYou()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _userUnitOfWork.User.GetForYouPageData(userId);
            model.Educators = _unitOfWork.Educator.GetEducatorsAboutUsPage();
            return View(model);
        }

        [Route("profil-resmi-degistir")]
        public async Task<IActionResult> ChangeProfileImage(IFormFile ProfileImage)
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var fileName = StringHelpers.FormatForTag($"{user.Name} {user.Surname}");
                var dbPath = await _storageService.UploadFile(ProfileImage.OpenReadStream(), $"{fileName}-{ProfileImage.FileName}", "user-avatars");
                user.AvatarPath = dbPath;
                await _userManager.UpdateAsync(user);
                return RedirectToAction("Profile", "UserProfile");
            }
            catch (Exception)
            {
                throw;
            }


        }

        [Route("haftaya-ozel-kurslar")]
        public IActionResult GetEducationsOfTheWeeks(int week)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var model = _unitOfWork.Suggestions.GetEducationsOfTheWeek(week, userId);
                return Json(new ResponseData
                {
                    Success = true,
                    Data = model
                });
            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("kullanici-iletisim-bilgisi-guncelle")]
        public IActionResult UpdateUserContactInfo(UpdateUserContactInfoVm model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = ModelStateUtil.GetErrors(ModelState);
            }
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.UserId = userId;
                _userUnitOfWork.User.UpdateUserContactInfo(model);
                TempData["Success"] = "Bilgileriniz başarıyla güncellendi!";
                return RedirectToAction("AccountSettings", "UserProfile");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "İşleminiz gerçekleştirilemedi! Lütfen tekrar deneyiniz.";
                //todo log
                throw;
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("kullanici-kisisel-bilgiler-guncelle")]
        public IActionResult UpdateStudentInfo(UpdateStudentInfoVm model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = ModelStateUtil.GetErrors(ModelState);
            }
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.UserId = userId;
                _userUnitOfWork.User.UpdateStudentInfo(model);
                TempData["Success"] = "Bilgileriniz başarıyla güncellendi!";
                return RedirectToAction("AccountSettings", "UserProfile");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "İşleminiz gerçekleştirilemedi! Lütfen tekrar deneyiniz.";
                //todo log
                throw;
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("kullanici-bilgileri-guncelle")]
        public async Task<IActionResult> UpdateUserInfo(UpdateUserInfoVm model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = ModelStateUtil.GetErrors(ModelState);
            }
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (model.OldPassword != null && model.NewPassword != null && model.ConfirmNewPassword != null)
                {
                    if (model.NewPassword == model.ConfirmNewPassword)
                    {
                        var retVal = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                        if (retVal.Succeeded)
                        {

                        }
                    }
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.UserId = userId;
                if (model.ProfileImage != null)
                {

                    var fileName = StringHelpers.FormatForTag($"{user.Name} {user.Surname}");
                    var dbPath = await _storageService.UploadFile(model.ProfileImage.OpenReadStream(), $"{fileName}-{model.ProfileImage.FileName}", "user-avatars");
                    user.AvatarPath = dbPath;
                    await _userManager.UpdateAsync(user);
                }

                //_userUnitOfWork.User.UpdateUserInfo(model);
                TempData["Success"] = "Bilgileriniz başarıyla güncellendi!";
                return RedirectToAction("AccountSettings", "UserProfile");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "İşleminiz gerçekleştirilemedi! Lütfen tekrar deneyiniz.";
                //todo log
                throw;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("nbuy-egitim-bilgileri-guncelle")]
        public  IActionResult UpdateNbuyEducationInfos(UpdateNBUYEducationInfoVm model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = ModelStateUtil.GetErrors(ModelState);
                return RedirectToAction("AccountSettings", "UserProfile");
            }
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.UserId = userId;
                _userUnitOfWork.User.UpdateNbuyEducationInfo(model);
                
                TempData["Success"] = "Bilgileriniz başarıyla güncellendi!";
                return RedirectToAction("AccountSettings", "UserProfile");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "İşleminiz gerçekleştirilemedi! Lütfen tekrar deneyiniz.";
                //todo log
                throw;
            }

        }

    }
}
