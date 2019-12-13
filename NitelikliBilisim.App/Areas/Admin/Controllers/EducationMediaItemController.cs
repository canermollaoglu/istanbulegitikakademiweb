using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Areas.Admin.VmCreator.EducationMediaItems;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_media_items;
using NitelikliBilisim.Support;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EducationMediaItemController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly EducationMediaItemVmCreator _vmCreator;
        private readonly FileUploadManager _fileUploadManager;
        public EducationMediaItemController(IHostingEnvironment hostingEnvironment, UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _vmCreator = new EducationMediaItemVmCreator(_unitOfWork);
            _fileUploadManager = new FileUploadManager(hostingEnvironment, "jpg", "jpeg", "mp4");
        }
        [Route("admin/egitim-medya-yonetimi/{educationId}")]
        public IActionResult Manage(Guid? educationId)
        {
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
        public IActionResult AddMediaItem(AddMediaItemVm data)
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
                var filePath = _fileUploadManager.Upload("/uploads/media-items/", data.PostedFile.Base64Content, data.PostedFile.Extension, EnumSupport.GetDescription((EducationMediaType)data.MediaItemType).ToLower(), education.Name);
                _unitOfWork.EducationMedia.Insert(new Core.Entities.EducationMedia
                {
                    EducationId = data.EducationId,
                    MediaType = (EducationMediaType)data.MediaItemType,
                    FileUrl = filePath
                });
                return Json(new ResponseModel
                {
                    isSuccess = true
                });
            }
            catch (Exception ex)
            {
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
            _fileUploadManager.Delete(mediaItem.FileUrl);
            _unitOfWork.EducationMedia.Delete(mediaItemId.Value);
            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }
    }
}
