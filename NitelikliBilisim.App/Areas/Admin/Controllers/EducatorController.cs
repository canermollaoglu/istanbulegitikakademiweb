using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Areas.Admin.VmCreator.Educator;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.areas.admin.educator;
using NitelikliBilisim.Support.Text;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    //[Authorize]
    [Area("Admin")]
    public class EducatorController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly FileUploadManager _fileManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EducatorVmCreator _vmCreator;
        private readonly IStorageService _storageService;

        public EducatorController(IWebHostEnvironment hostingEnvironment, UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IStorageService storageService)
        {
            _hostingEnvironment = hostingEnvironment;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _fileManager = new FileUploadManager(_hostingEnvironment, "jpg", "jpeg");
            _vmCreator = new EducatorVmCreator(_unitOfWork);
            _storageService = storageService;
        }
        [Route("admin/egitmen-ekle")]
        public IActionResult Add()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducatorAdd");
            return View();
        }

        [HttpPost, Route("admin/egitmen-ekle")]
        public async Task<IActionResult> Add(AddPostVm data)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelStateUtil.GetErrors(ModelState);
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = errors
                });
            }

            try
            {
                //var dbPath = _fileManager.Upload("/uploads/educator-photos/", data.ProfilePhoto.Base64Content, data.ProfilePhoto.Extension, "profile-photo", $"{data.Name} {data.Surname}");
                var stream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.ProfilePhoto.Base64Content));
                var fileName = $"{data.Name} {data.Surname}".FormatForTag();
                var dbPath = await _storageService.UploadFile(stream, $"{fileName}.{data.ProfilePhoto.Extension}", "educator-photos");
                var userName = TextHelper.ConcatForUserName(data.Name, data.Surname);

                var count = _userManager.Users.Count(x => x.UserName.StartsWith(userName));
                var countText = count > 0 ? count.ToString() : "";
                var newUser = new ApplicationUser
                {
                    Name = data.Name,
                    Surname = data.Surname,
                    AvatarPath = dbPath,
                    Email = data.Email,
                    PhoneNumber = data.Phone,
                    UserName = $"{userName}{countText}"
                };
                var pwd = TextHelper.RandomPasswordGenerator(10);
                var res = await _userManager.CreateAsync(newUser, pwd);
                if (!res.Succeeded)
                {
                    _fileManager.Delete(dbPath);

                    return Json(new ResponseModel
                    {
                        isSuccess = false,
                        errors = res.Errors.Select(x => x.Description)
                    });
                }
                    
                res = await _userManager.AddToRoleAsync(newUser, "User");
                if (!res.Succeeded)
                {
                    _fileManager.Delete(dbPath);

                    return Json(new ResponseModel
                    {
                        isSuccess = false,
                        errors = res.Errors.Select(x => x.Description)
                    });
                }

                var newEducator = new Educator
                {
                    Id = newUser.Id,
                    Title = data.Title,
                    Biography = "?"
                };
                _unitOfWork.Educator.Insert(newEducator);

                if (data.SocialMedia != null)
                    _unitOfWork.EducatorSocialMedia.Insert(newEducator.Id, data.SocialMedia.Facebook, data.SocialMedia.Linkedin, data.SocialMedia.GooglePlus, data.SocialMedia.Twitter);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }

        [Route("admin/egitmenler")]
        public IActionResult List()
        {

            return View();
        }

        [Route("admin/get-educators")]
        public IActionResult GetEducators()
        {
            var educators = _vmCreator.GetEducators();
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = educators
            });
        }

        public IActionResult Update(Guid? educatorId)
        {
            if (!educatorId.HasValue)
                return Redirect("/admin/egitmenler");

            return View();
        }
    }
}