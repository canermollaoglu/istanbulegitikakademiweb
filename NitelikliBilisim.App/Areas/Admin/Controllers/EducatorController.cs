﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
using NitelikliBilisim.Core.ViewModels.areas.admin.educator;
using NitelikliBilisim.Support.Text;

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
        public EducatorController(IWebHostEnvironment hostingEnvironment, UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _hostingEnvironment = hostingEnvironment;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _fileManager = new FileUploadManager(_hostingEnvironment, "jpg", "jpeg");
            _vmCreator = new EducatorVmCreator(_unitOfWork);
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
                var dbPath = _fileManager.Upload("/uploads/educator-photos/", data.ProfilePhoto.Base64Content, data.ProfilePhoto.Extension, "profile-photo", $"{data.Name} {data.Surname}");

                var userName = TextHelper.ConcatForUserName(data.Name, data.Surname);

                var count = _userManager.Users.Where(x => x.UserName.StartsWith(userName)).Count();
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
        [Route("admin/egitmen-guncelle/{educatorId}")]
        public IActionResult Update(Guid? educatorId)
        {
            if (!educatorId.HasValue)
                return Redirect("/admin/egitmenler");
            var educator = _unitOfWork.Educator.Get(x => x.Id == educatorId.ToString(), null, x => x.User).First();
            var model = new UpdateGetVm
            {
                Id = Guid.Parse(educator.Id),
                Title = educator.Title,
                Name = educator.User.Name,
                Surname = educator.User.Surname,
                Phone = educator.User.PhoneNumber,
                Email = educator.User.Email
            };
            return View(model);
        }
        [HttpPost, Route("admin/egitmen-guncelle")]
        public IActionResult Update(UpdatePostNewVm data)
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
            var educator = _unitOfWork.Educator.Get(x => x.Id == data.EducatorId.ToString(), null, x => x.User).First();
            educator.Title = data.Title;
            educator.User.Name = data.Name;
            educator.User.Surname = data.Surname;
            educator.User.PhoneNumber = data.Phone;
            educator.User.Email = data.Email;
            _unitOfWork.Educator.Update(educator);
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Eğitmen başarıyla güncellenmiştir"
            });
        }
    }
}