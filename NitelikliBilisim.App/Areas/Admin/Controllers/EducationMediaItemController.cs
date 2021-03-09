using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using NitelikliBilisim.App.Areas.Admin.VmCreator.EducationMediaItems;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_media_items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class EducationMediaItemController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly EducationMediaItemVmCreator _vmCreator;
        private readonly FileUploadManager _fileManager;
        private readonly IStorageService _storage;
        public EducationMediaItemController(IWebHostEnvironment hostingEnvironment, UnitOfWork unitOfWork, IStorageService storage)
        {
            _unitOfWork = unitOfWork;
            _fileManager = new FileUploadManager(hostingEnvironment, "jpg", "jpeg", "png");
            _storage = storage;
            _vmCreator = new EducationMediaItemVmCreator(_unitOfWork, _storage);
        }
        [Route("admin/egitim-medya-yonetimi/{educationId}")]
        public IActionResult Manage(Guid? educationId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationMediaItem");
            if (educationId == null)
                return Redirect("/");
            var model = _vmCreator.CreateManageVm(educationId.Value);

            return View(model);
        }
        [Route("admin/get-education-media-items/{educationId}")]
        public IActionResult GetEducationMediaItems(Guid? educationId)
        {
            if (educationId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Eğitimin medyalarını getirirken bir hata oluştu" }
                });

            var model = _vmCreator.CreateEducationMediaItemsVm(educationId.Value);

            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });
        }
        [HttpPost, Route("admin/add-education-media-item")]
        public async Task<IActionResult> AddMediaItem(AddMediaItemVm data)
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

            var education = _unitOfWork.Education.GetById(data.EducationId);
            var mediaItem = _unitOfWork.EducationMedia.Get(x => x.EducationId == data.EducationId && x.MediaType == (EducationMediaType)data.MediaItemType).FirstOrDefault();
            if (mediaItem != null)
            {
                await _storage.DeleteFile(Path.GetFileName(mediaItem.FileUrl), Path.GetDirectoryName(mediaItem.FileUrl));
                _unitOfWork.EducationMedia.Delete(mediaItem);
            }

            var mediaStream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.PostedFile.Base64Content));
            var mediaFileName = $"{StringHelpers.FormatForTag(education.Name)}-{EnumHelpers.GetDescription((EducationMediaType)data.MediaItemType).ToLower()}";
            var mediaPath = await _storage.UploadFile(mediaStream, $"{mediaFileName}.{data.PostedFile.Extension.ToLower()}", "media-items");

            _unitOfWork.EducationMedia.Insert(new EducationMedia
            {
                EducationId = data.EducationId,
                MediaType = (EducationMediaType)data.MediaItemType,
                FileUrl = mediaPath
            });

            return Json(new ResponseModel
            {
                isSuccess = true
            });

        }

        [Route("admin/delete-education-media-item/{mediaItemId}")]
        public IActionResult DeleteMediaItem(Guid? mediaItemId)
        {
            if (mediaItemId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Eğitimin medyasını silerken bir hata oluştu" }
                });
            var mediaItem = _unitOfWork.EducationMedia.GetById(mediaItemId.Value);

            if (mediaItem == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Eğitimin medyası silindi." }
                });

            _storage.DeleteFile(Path.GetFileName(mediaItem.FileUrl), Path.GetDirectoryName(mediaItem.FileUrl));
            _unitOfWork.EducationMedia.Delete(mediaItemId.Value);


            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }
    }
}
