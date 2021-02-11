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

        [Route("admin/one-cikarilan-yorum/guncelle")]
        public IActionResult Update(Guid id)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminFeaturedCommentUpdate");
            var featuredComment = _unitOfWork.FeaturedComment.GetById(id);
            featuredComment.PreviewImageFileUrl = _storage.BlobUrl + featuredComment.PreviewImageFileUrl;
            return View(featuredComment);
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

            var previewImageStream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.PreviewImageFile.Base64Content));
            var previewImageFileName = $"{StringHelpers.FormatForTag(data.Name)}-preview";
            var previewImagePath = await _storage.UploadFile(previewImageStream, $"{previewImageFileName}.{data.PreviewImageFile.Extension.ToLower()}", "featured-comment-medias");

            var featuredComment = new FeaturedComment
            {
                Name = data.Name,
                Surname = data.Surname,
                Title = data.Title,
                Content = data.Comment,
                FileUrl = data.VideoUrl,
                PreviewImageFileUrl = previewImagePath
            };

            _unitOfWork.FeaturedComment.Insert(featuredComment);

            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Yorum başarıyla eklenmiştir"
            });

        }


        [Route("admin/one-cikarilan-yorum/guncelle")]
        [HttpPost]
        public async Task<IActionResult> Update(FeaturedCommentUpdatePostVm data)
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

            var featuredComment = _unitOfWork.FeaturedComment.GetById(data.Id);

            featuredComment.Name = data.Name;
            featuredComment.Surname = data.Surname;
            featuredComment.Title = data.Title;
            featuredComment.Content = data.Comment;
            featuredComment.FileUrl = data.VideoUrl;
            if (data.PreviewImageFile.Base64Content != null)
            {
                var previewImageStream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.PreviewImageFile.Base64Content));
                var previewImageFileName = $"{StringHelpers.FormatForTag(data.Name)}-preview";
                var previewImagePath = await _storage.UploadFile(previewImageStream, $"{previewImageFileName}.{data.PreviewImageFile.Extension.ToLower()}", "featured-comment-medias");
                featuredComment.PreviewImageFileUrl = previewImagePath;
            }

            _unitOfWork.FeaturedComment.Update(featuredComment);

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
