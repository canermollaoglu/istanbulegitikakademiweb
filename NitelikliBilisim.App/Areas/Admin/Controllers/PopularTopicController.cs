using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.areas.admin.popular_topic;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class PopularTopicController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly FileUploadManager _fileManager;
        private readonly IStorageService _storage;
        public PopularTopicController(UnitOfWork unitOfWork,IStorageService storage, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _storage = storage;
            _fileManager = new FileUploadManager(hostingEnvironment, "jpg", "jpeg", "png");
        }

        [Route("admin/populer-konu-basliklari")]
        public IActionResult Index()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminPopularTopics");
            return View();
        }
        [Route("admin/populer-konu-basligi-ekle")]
        public IActionResult Add()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminPopularTopicAdd");
            return View();
        }

        [Route("admin/populer-konu-basligi-guncelle/{id}")]
        public IActionResult Update(Guid id)
        {
            var detail = _unitOfWork.PopularTopic.GetById(id);
            detail.IconUrl = _storage.BlobUrl + detail.IconUrl;
            detail.BackgroundUrl = _storage.BlobUrl + detail.BackgroundUrl;
            return View(detail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PopularTopicUpdatePostVm data)
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

            var entity = _unitOfWork.PopularTopic.GetById(data.Id);
            entity.ShortTitle = data.ShortTitle;
            entity.Title = data.Title;
            entity.TargetUrl = data.TargetUrl;
            entity.Description = data.Description;

            if (!string.IsNullOrEmpty(data.BackgroundImage.Base64Content))
            {
                var bgStream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.BackgroundImage.Base64Content));
                var bgImageFileName = $"{StringHelpers.FormatForTag(data.Title)}-bg";
                var bgUrl = await _storage.UploadFile(bgStream, $"{bgImageFileName}.{data.BackgroundImage.Extension.ToLower()}", "popular-topic-images");
                entity.BackgroundUrl = bgUrl;
            }
            if (!string.IsNullOrEmpty(data.IconImage.Base64Content))
            {
                var iconStream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.IconImage.Base64Content));
                var iconImageFileName = $"{StringHelpers.FormatForTag(data.Title)}-icon";
                var iconUrl = await _storage.UploadFile(iconStream, $"{iconImageFileName}.{data.IconImage.Extension.ToLower()}", "popular-topic-images");
                entity.IconUrl = iconUrl;
            }
            _unitOfWork.PopularTopic.Update(entity);
            return Json(new ResponseModel
            {
                isSuccess = true,
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/populer-konu-basligi-ekle")]
        public async Task<IActionResult> Add(PopularTopicAddPostVm data)
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
            var iconStream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.IconImage.Base64Content));
            var iconImageFileName = $"{StringHelpers.FormatForTag(data.Title)}-icon";
            var iconUrl = await _storage.UploadFile(iconStream, $"{iconImageFileName}.{data.IconImage.Extension.ToLower()}", "popular-topic-images");

            var bgStream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.BackgroundImage.Base64Content));
            var bgImageFileName = $"{StringHelpers.FormatForTag(data.Title)}-bg";
            var bgUrl = await _storage.UploadFile(bgStream, $"{bgImageFileName}.{data.BackgroundImage.Extension.ToLower()}", "popular-topic-images");

            var newPopularTopic = new PopularTopic
            {
                Title = data.Title,
                ShortTitle = data.ShortTitle,
                TargetUrl = data.TargetUrl,
                Description = data.Description,
                IconUrl = iconUrl,
                BackgroundUrl = bgUrl
            };
            _unitOfWork.PopularTopic.Insert(newPopularTopic);
            return Json(new ResponseModel
            {
                isSuccess = true,
            });
        }

        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            _unitOfWork.PopularTopic.Delete(id);
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Silme işlemi başarılı"
            });
        }

    }
}
