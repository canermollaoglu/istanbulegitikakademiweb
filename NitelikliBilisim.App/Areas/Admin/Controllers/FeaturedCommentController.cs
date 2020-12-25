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
using NitelikliBilisim.Core.ViewModels.areas.admin.featured_comment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class FeaturedCommentController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly FileUploadManager _fileManager;
        private readonly IStorageService _storage;
        public FeaturedCommentController(IWebHostEnvironment hostingEnvironment, UnitOfWork unitOfWork, IStorageService storage)
        {
            _unitOfWork = unitOfWork;
            _fileManager = new FileUploadManager(hostingEnvironment, "jpg", "jpeg", "png");
            _storage = storage;
        }

        [Route("admin/one-cikarilan-yorum/ekle")]
        public IActionResult Add()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminFeaturedCommentAdd");
            return View();
        }

        [Route("admin/one-cikarilan-yorum/ekle")]
        [HttpPost]
        public async Task<IActionResult> Add(FeaturedCommentAddPostVm data)
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
            var previewStream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.PreviewFile.Base64Content));
            var previewFileName = $"{StringHelpers.FormatForTag(data.Name)}-preview";
            var previewPath = await _storage.UploadFile(previewStream, $"{previewFileName}.{data.PreviewFile.Extension.ToLower()}", "featured-comment-medias");

            var previewImageStream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.PreviewImageFile.Base64Content));
            var previewImageFileName = $"{StringHelpers.FormatForTag(data.Name)}-preview";
            var previewImagePath = await _storage.UploadFile(previewImageStream, $"{previewImageFileName}.{data.PreviewImageFile.Extension.ToLower()}", "featured-comment-medias");

            var featuredComment = new FeaturedComment
            {
                Name = data.Name,
                Surname = data.Surname,
                Title= data.Title,
                Content = data.Comment,
                FileUrl = previewPath,
                PreviewImageFileUrl = previewImagePath
            };

            _unitOfWork.FeaturedComment.Insert(featuredComment);

            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Yorum başarıyla eklenmiştir"
            });

        }

        [Route("admin/one-cikarilan-yorumlar")]
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminFeaturedCommentList");
            return View();
        }

        [HttpGet]
        [Route("admin/one-cikarilan-yorumlar/sil")]
        public IActionResult Delete(Guid featuredCommentId)
        {
            try
            {
                _unitOfWork.FeaturedComment.Delete(featuredCommentId);
            }
            catch (Exception)
            {
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    message = "Silme işlemi gerçekleşmedi! Bu işlem için sistem yöneticiniz ile iletişime geçiniz."
                });
            }

            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Silme işlemi başarılı"
            });

        }

    }
}
