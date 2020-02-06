using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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
using NitelikliBilisim.Support.Enums;
using NitelikliBilisim.Support.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    //[Authorize]
    [Area("Admin")]
    public class EducationMediaItemController : TempSecurityController
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
            try
            {
                var education = _unitOfWork.Education.GetById(data.EducationId);

                var mediaStream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.PostedFile.Base64Content));
                var mediaFileName = $"{education.Name.FormatForTag()}-{EnumSupport.GetDescription((EducationMediaType)data.MediaItemType).ToLower()}";
                var mediaPath = await _storage.UploadFile(mediaStream, $"{mediaFileName}.{data.PostedFile.Extension.ToLower()}", "media-items");
              
                _unitOfWork.EducationMedia.Insert(new EducationMedia
                {
                    EducationId = data.EducationId,
                    MediaType = (EducationMediaType)data.MediaItemType,
                    FileUrl = mediaPath
                });

                _unitOfWork.Education.CheckEducationState(data.EducationId);

                return Json(new ResponseModel
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Dosya yüklenirken hata oluştu" }
                });
            }
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
                    errors = new List<string> { "Eğitimin medyasını silerken bir hata oluştu" }
                });
                
            _fileManager.Delete(mediaItem.FileUrl);
            _unitOfWork.EducationMedia.Delete(mediaItemId.Value);

            _unitOfWork.Education.CheckEducationState(mediaItem.EducationId);

            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }
    }
}
