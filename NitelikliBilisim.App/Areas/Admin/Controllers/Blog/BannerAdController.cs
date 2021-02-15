using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities.blog;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.areas.admin.blog.bannerAd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.Blog
{
    public class BannerAdController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly FileUploadManager _fileManager;
        private readonly IStorageService _storage;
        public BannerAdController(UnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment, IStorageService storage)
        {
            _unitOfWork = unitOfWork;
            _fileManager = new FileUploadManager(hostingEnvironment, "jpg", "jpeg", "png");
            _storage = storage;
        }

        [Route("admin/blog-reklam-listesi")]
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminBlogBannerAd");
            return View();
        }

        [Route("admin/blog/banner-ekle")]
        public IActionResult Add()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminBlogBannerAdAdd");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/blog/banner-ekle")]
        public async Task<IActionResult> Add(AddBannerAdVm data)
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
                var featuredImageStream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.FeaturedImage.Base64Content));
                var featuredImageFileName = $"{StringHelpers.FormatForTag(data.Title1 + data.Title2)}-featured";
                var featuredImagePath = await _storage.UploadFile(featuredImageStream, $"{featuredImageFileName}.{data.FeaturedImage.Extension.ToLower()}", "banner-ads-images");

                var newBannerAd = new BannerAd
                {
                    Title1 = data.Title1,
                    Title2 = data.Title2,
                    Code = data.Code,
                    IconUrl = data.IconUrl,
                    Content = data.Content,
                    ImageUrl = featuredImagePath,
                    RelatedApplicationUrl = data.RelatedApplicationUrl
                };

                _unitOfWork.BannerAds.Insert(newBannerAd);

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
                    errors = new List<string> { ex.Message }
                });
            }
           
           
        }

        [HttpGet]
        [Route("admin/blog/banner-guncelle")]
        public IActionResult Update(Guid bannerAdId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminBlogBannerAdUpdate");
            var banner = _unitOfWork.BannerAds.GetById(bannerAdId);
            UpdateBannerAdGetVm model = new UpdateBannerAdGetVm
            {
                Id = banner.Id,
                Title1 = banner.Title1,
                Title2 = banner.Title2,
                IconUrl= banner.IconUrl,
                ImageUrl = _storage.BlobUrl+banner.ImageUrl,
                Content = banner.Content,
                Code = banner.Code,
                RelatedApplicationUrl = banner.RelatedApplicationUrl
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/blog/banner-guncelle")]
        public async Task<IActionResult> Update(UpdateBannerAdPostVm data)
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
                var bannerAd = _unitOfWork.BannerAds.GetById(data.Id);
                if (!string.IsNullOrEmpty(data.FeaturedImage.Base64Content))
                {
                    var featuredImageStream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.FeaturedImage.Base64Content));
                    var featuredImageFileName = $"{StringHelpers.FormatForTag(data.Title1 + data.Title2)}-featured";
                    var featuredImagePath = await _storage.UploadFile(featuredImageStream, $"{featuredImageFileName}.{data.FeaturedImage.Extension.ToLower()}", "banner-ads-images");
                    bannerAd.ImageUrl= featuredImagePath;
                }
                bannerAd.Title1 = data.Title1;
                bannerAd.Title2 = data.Title2;
                bannerAd.IconUrl = data.IconUrl;
                bannerAd.Content = data.Content;
                bannerAd.RelatedApplicationUrl = data.RelatedApplicationUrl;
                bannerAd.Code = data.Code;
                int retVal = _unitOfWork.BannerAds.Update(bannerAd);

                return Json(new ResponseModel
                {
                    isSuccess = true,
                    message = "Yazı başarıyla güncellenmiştir."
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

        public IActionResult Delete(Guid? bannerAdId)
        {
            if (bannerAdId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Yazı silinirken bir hata oluştu." }
                });
            try
            {
                var bannerAd = _unitOfWork.BannerAds.GetById(bannerAdId.Value);
                string folderName = Path.GetDirectoryName(bannerAd.ImageUrl);
                string fileName = Path.GetFileName(bannerAd.ImageUrl);
                _storage.DeleteFile(fileName, folderName);
                _unitOfWork.BannerAds.Delete(bannerAdId.Value);
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
                    errors = new List<string> { "Hata : " + ex.Message }
                });
            }

        }

    }
}
