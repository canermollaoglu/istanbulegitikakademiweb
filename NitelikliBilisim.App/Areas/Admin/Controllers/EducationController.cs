﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Areas.Admin.Models.Education;
using NitelikliBilisim.App.Areas.Admin.VmCreator.Education;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EducationController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly EducationVmCreator _vmCreator;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly FileUploadManager _fileUploadManager;
        public EducationController(UnitOfWork unitOfWork, IHostingEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _vmCreator = new EducationVmCreator(_unitOfWork);
            _hostingEnvironment = hostingEnvironment;
            _fileUploadManager = new FileUploadManager(hostingEnvironment, "mp4", "jpg", "jpeg", "png");
        }
        [Route("admin/egitim-ekle")]
        public IActionResult Add()
        {
            var model = _vmCreator.CreateAddGetVm();
            return View(model);
        }

        [HttpPost, Route("admin/egitim-ekle")]
        public IActionResult Add(AddPostVm data)
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

            var bannerPath = _fileUploadManager.Upload($"/uploads/media-items/", data.BannerFile.Base64Content, data.BannerFile.Extension, "banner", data.Name);
            var banner = new EducationMedia
            {
                FileUrl = bannerPath,
                MediaType = EducationMediaType.Banner
            };

            var previewPath = _fileUploadManager.Upload($"/uploads/media-items/", data.PreviewFile.Base64Content, data.PreviewFile.Extension, "preview", data.Name);
            var preview = new EducationMedia
            {
                FileUrl = previewPath,
                MediaType = data.PreviewFile.Extension == "mp4" ? EducationMediaType.PreviewVideo : EducationMediaType.PreviewPhoto
            };

            var education = new Education
            {
                Name = data.Name,
                Description = data.Description,
                Level = (EducationLevel)data.EducationLevel.Value,
                NewPrice = data.Price.Value,
                Days = data.Days.Value,
                HoursPerDay = data.HoursPerDay.Value,
                CategoryId = data.CategoryId
            };

            _unitOfWork.Education.Insert(education, data.TagIds, new List<EducationMedia> { banner, preview });

            _unitOfWork.Education.CheckEducationState(education.Id);

            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Eğitim başarıyla eklenmiştir"
            });
        }

        [Route("admin/egitimler")]
        public IActionResult List(int page = 0)
        {
            var model = _vmCreator.CreateListGetVm(page);
            return View(model);
        }

        [Route("admin/egitim-guncelle/{educationId}")]
        public IActionResult Update(Guid? educationId)
        {
            if (!educationId.HasValue)
                return Redirect("/admin/egitimler");

            var model = _vmCreator.CreateUpdateGetVm(educationId.Value);
            return View(model);
        }
        [HttpPost, Route("admin/egitim-guncelle")]
        public IActionResult Update(UpdatePostVm data)
        {
            if (!ModelState.IsValid || data.TagIds.Count == 0)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = ModelStateUtil.GetErrors(ModelState)
                });

            _vmCreator.SendVmToUpdate(data);

            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }
    }
}