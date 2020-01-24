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
using NitelikliBilisim.Core.Services;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Support.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    //[Authorize]
    [Area("Admin")]
    public class EducationController : Controller
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
            var bannerFileName = $"{data.Name.FormatForTag()}-banner";
            var bannerPath = await _storage.UploadFile(bannerStream, $"{bannerFileName}.{data.BannerFile.Extension.ToLower()}", "media-items");
            var banner = new EducationMedia
            {
                FileUrl = bannerPath,
                MediaType = EducationMediaType.Banner
            };

            //var previewPath = _fileManager.Upload($"/uploads/media-items/", data.PreviewFile.Base64Content, data.PreviewFile.Extension, "preview", data.Name);
            var previewStream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.PreviewFile.Base64Content));
            var previewFileName = $"{data.Name.FormatForTag()}-preview";
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
                NewPrice = data.Price.GetValueOrDefault(),
                Days = data.Days.GetValueOrDefault(),
                HoursPerDay = data.HoursPerDay.GetValueOrDefault(),
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

        [Route("admin/egitmen-ata/{educationId}")]
        public IActionResult ManageAssignEducators(Guid? educationId)
        {
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
    }
}