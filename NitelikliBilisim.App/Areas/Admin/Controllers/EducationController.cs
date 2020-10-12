﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MUsefulMethods;
using NitelikliBilisim.App.Areas.Admin.Models.Education;
using NitelikliBilisim.App.Areas.Admin.VmCreator.Education;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.HelperVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class EducationController : TempSecurityController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly EducationVmCreator _vmCreator;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly FileUploadManager _fileManager;
        private readonly IStorageService _storage;
        public EducationController(UnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment, IStorageService storage)
        {
            _unitOfWork = unitOfWork;
            _vmCreator = new EducationVmCreator(_unitOfWork);
            _hostingEnvironment = hostingEnvironment;
            _fileManager = new FileUploadManager(hostingEnvironment, "mp4", "jpg", "jpeg", "png");
            _storage = storage;
        }
        [Route("admin/egitim-ekle")]
        public IActionResult Add()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationAdd");
            var model = _vmCreator.CreateAddGetVm();
            return View(model);
        }

        [HttpPost, Route("admin/egitim-ekle")]
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

            //var bannerPath = _fileUploadManager.Upload($"/uploads/media-items/", data.BannerFile.Base64Content, data.BannerFile.Extension, "banner", data.Name);
            var bannerStream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.BannerFile.Base64Content));
            var bannerFileName = $"{StringHelpers.FormatForTag(data.Name)}-banner";
            var bannerPath = await _storage.UploadFile(bannerStream, $"{bannerFileName}.{data.BannerFile.Extension.ToLower()}", "media-items");
            var banner = new EducationMedia
            {
                FileUrl = bannerPath,
                MediaType = EducationMediaType.Banner
            };

            //var previewPath = _fileManager.Upload($"/uploads/media-items/", data.PreviewFile.Base64Content, data.PreviewFile.Extension, "preview", data.Name);
            var previewStream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.PreviewFile.Base64Content));
            var previewFileName = $"{StringHelpers.FormatForTag(data.Name)}-preview";
            var previewPath = await _storage.UploadFile(previewStream, $"{previewFileName}.{data.PreviewFile.Extension.ToLower()}", "media-items");
            var preview = new EducationMedia
            {
                FileUrl = previewPath,
                MediaType = data.PreviewFile.Extension == "mp4" ? EducationMediaType.PreviewVideo : EducationMediaType.PreviewPhoto
            };

            var education = new Education
            {
                Name = data.Name,
                Description = data.Description,
                Description2 = data.Description2,
                Level = (EducationLevel)data.EducationLevel.GetValueOrDefault(),
                Days = data.Days.GetValueOrDefault(),
                HoursPerDay = data.HoursPerDay.GetValueOrDefault(),
                CategoryId = data.CategoryId
            };

            _unitOfWork.Education.Insert(education, data.Tags, new List<EducationMedia> { banner, preview });

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
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationList");
            return View();
        }

        [Route("admin/get-education-list")]
        public IActionResult GetList(int page = 0)
        {
            var model = _vmCreator.CreateListGetVm(page);

            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }


        [Route("admin/egitim-guncelle/{educationId}")]
        public IActionResult Update(Guid? educationId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationUpdate");
            if (!educationId.HasValue)
                return Redirect("/admin/egitimler");

            var model = _vmCreator.CreateUpdateGetVm(educationId.Value);
            return View(model);
        }
        [HttpPost, Route("admin/egitim-guncelle")]
        public IActionResult Update(UpdatePostVm data)
        {
            if (!ModelState.IsValid || data.Tags.Length== 0)
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

        [Route("admin/egitmen-ata/{educationId}")]
        public IActionResult ManageAssignEducators(Guid? educationId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationManageAssignEducators");
            if (educationId == null)
                return Redirect("/admin/egitimler");
            var model = _vmCreator.CreateManageAssignEducatorsVm(educationId.Value);

            return View(model);
        }
        [Route("admin/assign-educators")]
        public IActionResult AssignEducators(Core.ViewModels.areas.admin.educator.ManageAssignEducatorsPostVm data)
        {
            _unitOfWork.Bridge_EducationEducator.Insert(data);
            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }

        [Route("admin/get-assigned-educators/{educationId}")]
        public IActionResult GetEducators(Guid? educationId)
        {
            if (!educationId.HasValue)
                return Json(new ResponseModel
                {
                    isSuccess = false
                });

            return Json(new ResponseModel
            {
                isSuccess = true,
                data = _unitOfWork.Bridge_EducationEducator.GetAssignedEducators(educationId.Value)
            });
        }
        [Route("admin/delete-egitmen-ata")]
        public IActionResult DeleteEducator(Guid? educationId, Guid educatorId)
        {
            if (educationId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Eğitmen atama silerken bir hata oluştu" }
                });

            _unitOfWork.Bridge_EducationEducator.Delete(educationId.Value, educatorId);
            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }

        [Route("admin/education-list-fill-select")]
        public IActionResult EducationListFillSelect()
        {
            try
            {
                List<SelectListItem> educationList = _unitOfWork.Education.Get().Select(e => new SelectListItem
                {
                    Text = e.Name,
                    Value = e.Id.ToString()
                }).ToList();

                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = educationList
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata" + ex.Message }
                }); ;
            }
            
        }


        [Route("admin/get-education-levels/")]
        public IActionResult GetEducationLevelEnums()
        {
            try
            {
                EnumItemVm[] retVal = EnumHelpers.ToKeyValuePair<EducationLevel>().Select(x =>
            new EnumItemVm { Key = x.Key, Value = x.Value }).ToArray();
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data = retVal
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { ex.Message }
                });
            }

        }
    }
}