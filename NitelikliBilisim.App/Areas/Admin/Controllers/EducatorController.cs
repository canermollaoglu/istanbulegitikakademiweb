﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.ViewModels.areas.admin.educator;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EducatorController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly FileUploadManager _fileManager;
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public EducatorController(IHostingEnvironment hostingEnvironment, UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _hostingEnvironment = hostingEnvironment;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _fileManager = new FileUploadManager(_hostingEnvironment, "jpg", "jpeg");
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

                var newUser = new ApplicationUser
                {
                    Name = data.Name,
                    Surname = data.Surname,
                    AvatarPath = dbPath,
                    UserName = data.Email,
                    Email = data.Email
                };

                await _userManager.CreateAsync(newUser, "password");
                //await _userManager.AddToRoleAsync(newUser, "User");

                _unitOfWork.Educator.Insert(new Educator
                {
                    Id = newUser.Id,
                    Title = "?",
                    Biography = "?"
                });
            }
            catch (Exception ex)
            {

            }

            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }
    }
}