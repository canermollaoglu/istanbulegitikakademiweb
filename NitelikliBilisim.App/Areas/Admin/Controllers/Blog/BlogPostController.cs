using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities.blog;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.areas.admin.blog.blogpost;
using NitelikliBilisim.Support.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class BlogPostController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly FileUploadManager _fileManager;
        private readonly IStorageService _storage;
        public BlogPostController(UnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment, IStorageService storage)
        {
            _unitOfWork = unitOfWork;
            _fileManager = new FileUploadManager(hostingEnvironment, "jpg", "jpeg", "png");
            _storage = storage;
        }

        [HttpGet]
        public IActionResult List()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Preview(Guid postId)
        {
            var post = _unitOfWork.BlogPost.GetByIdWithCategoryandTags(postId);
            BlogPostGetVM model = new BlogPostGetVM();
            model.Title = post.Title;
            model.FeaturedImageUrl = _storage.DownloadFile(Path.GetFileName(post.FeaturedImageUrl), "blog-featured-images").Result;
            model.Category = post.Category;
            model.Content = post.Content;
            model.Id = post.Id;
            model.CreatedDate = post.UpdatedDate ??= post.CreatedDate;
            model.ReadingTime = post.ReadingTime;

            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = new BlogPostAddGetVM
            {
                BlogCategories = _unitOfWork.BlogCategory.Get().ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(BlogPostAddVM data)
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
            var featuredImageStream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.FeaturedImage.Base64Content));
            var featuredImageFileName = $"{data.Title.FormatForTag()}-featured";
            var featuredImagePath = await _storage.UploadFile(featuredImageStream, $"{featuredImageFileName}.{data.FeaturedImage.Extension.ToLower()}", "blog-featured-images");

            var newBlogPost = new BlogPost
            {
                Title = data.Title,
                CategoryId = data.CategoryId,
                Content = data.Content,
                FeaturedImageUrl = featuredImagePath,
                IsActive = true,
                ReadingTime = CalculateReadingTime(data.Content)
            };

            _unitOfWork.BlogPost.Insert(newBlogPost, data.Tags);

            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Yazı başarıyla eklenmiştir"
            });

        }

        [HttpPost]
        public async Task<IActionResult> AddPostImage(IFormFile upload)
        {
            try
            {
                var featuredImageStream = new MemoryStream(ConvertToBytes(upload));
                if (featuredImageStream != null)
                {
                    var featuredImageFileName = $"{upload.FileName.FormatForTag()}-blogpostimage";
                    var featuredImagePath = await _storage.UploadFile(featuredImageStream, $"{featuredImageFileName}.{GetExtension(upload.ContentType).ToLower()}", "blog-post-images");

                    var url = _storage.DownloadFile(Path.GetFileName(featuredImagePath), "blog-post-images").Result;
                    return Json(new { uploaded = true, url });
                }
                else
                {
                    return Json(new { uploaded = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { uploaded = false });
                Console.Write(ex.Message);
            }
        }

      

        [HttpGet]
        public IActionResult UpdatePost(Guid? postId)
        {
            return View();
        }
        //[HttpPost]
        //public IActionResult Update()
        //{

        //}

        public IActionResult Delete(Guid? postId)
        {
            if (postId == null)
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Yazı silinirken bir hata oluştu." }
                });
            try
            {
                _unitOfWork.BlogPost.Delete(postId.Value);

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


        #region Helper Methods 
        /// <summary>
        /// Kelime sayısını 200 e bölerek ortalama okunma süresini döner.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private int CalculateReadingTime(string content)
        {
            int wordCount = Regex.Matches(content, @"[\S]+").Count();
            return wordCount / 200;
        }
        /// <summary>
        /// Verilen content type ait extension u döner
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private string GetExtension(string contentType)
        {

            Dictionary<string, string> extensionLookup = new Dictionary<string, string>()
            {
                {"image/jpeg", ".jpeg"},
                {"image/png", ".png"},
            };
            return extensionLookup[contentType];
        }
        /// <summary>
        /// IFromFile tipindeki nesneyi byte array olarak döner
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private byte[] ConvertToBytes(IFormFile image)
        {
            byte[] CoverImageBytes = null;
            BinaryReader reader = new BinaryReader(image.OpenReadStream());
            CoverImageBytes = reader.ReadBytes((int)image.Length);
            return CoverImageBytes;
        }
        #endregion
    }
}
