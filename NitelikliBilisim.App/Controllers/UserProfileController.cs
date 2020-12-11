using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Filters;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ComplexTypes;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Services.Abstracts;

namespace NitelikliBilisim.App.Controllers
{
    [Authorize]
    public class UserProfileController : BaseController
    {
        private readonly UserUnitOfWork _userUnitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStorageService _storageService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly FileUploadManager _fileManager;
        public UserProfileController(UserUnitOfWork userUnitOfWork, UserManager<ApplicationUser> userManager, IStorageService storageService, IWebHostEnvironment hostingEnvironment)
        {
            _userUnitOfWork = userUnitOfWork;
            _userManager = userManager;
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

        [HttpPost, Route("profil/avatar-guncelle")]
        public async Task<IActionResult> UpdateUserAvatar(string base64Content, string extension)
        {
            if (!string.IsNullOrEmpty(base64Content))
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                var stream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(base64Content));
                var fileName = StringHelpers.FormatForTag($"{user.Name} {user.Surname}");
                var dbPath = await _storageService.UploadFile(stream, $"{fileName}.{extension}", "user-avatars");
                user.AvatarPath = dbPath;

                await _userManager.UpdateAsync(user);

                return Json(new ResponseData
                {
                    Success = true
                });
            }

            return Json(new ResponseData
            {
                Success = false
            });
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

        [Route("kuponlarim")]
        public IActionResult MyPromotions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = _userUnitOfWork.User.GetCustomerPromotions(userId);
            if (model == null)
                return Redirect($"/profil/{userId}");
            return Json(model);
        }

        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        public IActionResult Cancellation(Guid? ticketId)
        {
            if (!ticketId.HasValue)
                return Redirect($"/profil/{User.FindFirstValue(ClaimTypes.NameIdentifier)}");

            return View();
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
            return View();
        }


    }
}
