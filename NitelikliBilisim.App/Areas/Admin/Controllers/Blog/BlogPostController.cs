using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Managers;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities.blog;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.areas.admin.blog.blogpost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class BlogPostController : BaseController
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
        [Route("/admin/blog/liste")]
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminBlogPostList");
            return View();
        }

        [HttpGet]
        [Route("/admin/blog/onizle")]
        public IActionResult Preview(Guid postId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminBlogPostView");
            var post = _unitOfWork.BlogPost.GetByIdWithCategory(postId);
            BlogPostGetVM model = new BlogPostGetVM();
            model.Title = post.Title;
            model.FeaturedImageUrl = _storage.BlobUrl + post.FeaturedImageUrl;
            model.Category = post.Category;
            model.Content = post.Content;
            model.Id = post.Id;
            model.CreatedDate = post.UpdatedDate ??= post.CreatedDate;
            model.ReadingTime = post.ReadingTime;

            if (post.Content.Contains("[##") && post.Content.Contains("##]"))
            {
                var index = post.Content.IndexOf("[##");
                var lastChar = post.Content.IndexOf("##]", index);

                var code = post.Content.Substring(index + 3, lastChar - index - 3);
                var banner = _unitOfWork.BannerAds.GetBannerByCode(code);
                if (banner != null)
                {
                    var htmlContent = "<a href=\"#0\" class=\"custom-banner white\">";
                    htmlContent += "<div class=\"custom-banner__icon\">";
                    htmlContent += "<span class=\"icon-outer\">";
                    htmlContent += "<svg class=\"icon\">";
                    htmlContent += $"<use xlink:href=\"assets/img/icons.svg#{banner.IconUrl}\"></use>";
                    htmlContent += "</svg>";
                    htmlContent += "</span></div>";
                    htmlContent += "<div class=\"custom-banner__cnt\">";
                    htmlContent += $"<div class=\"custom-banner__title\">{banner.Title1}<span class=\"title--blue\">{banner.Title2}</span></div>";
                    htmlContent += $"<div class=\"custom-banner__txt\">{banner.Content}</div>";
                    htmlContent += "</div>";
                    htmlContent += "<div class=\"custom-banner__img\">";
                    htmlContent += $"<img src=\"{_storage.BlobUrl+banner.ImageUrl}\">";
                    htmlContent += "</div></a>";
                    model.Content= post.Content.Replace("[##" + code + "##]", htmlContent);
                }
            }

            return View(model);
        }

        [HttpGet]
        [Route("/admin/blog/yazi-ekle")]
        public IActionResult Add()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminBlogPostAdd");
            var bannerAds = _unitOfWork.BannerAds.Get().ToList();
            foreach (var banner in bannerAds)
            {
                banner.ImageUrl = _storage.BlobUrl + banner.ImageUrl;
            }
            var model = new BlogPostAddGetVM
            {
                BlogCategories = _unitOfWork.BlogCategory.Get().ToList(),
                BannerAds = bannerAds
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
            var featuredImageFileName = $"{StringHelpers.FormatForTag(data.Title)}-featured";
            var featuredImagePath = await _storage.UploadFile(featuredImageStream, $"{featuredImageFileName}.{data.FeaturedImage.Extension.ToLower()}", "blog-featured-images");

            var newBlogPost = new BlogPost
            {
                Title = data.Title,
                SeoUrl = data.SeoUrl,
                CategoryId = data.CategoryId,
                SummaryContent = data.SummaryContent,
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
            catch
            {
                return Json(new { uploaded = false });
            }
        }



        [HttpGet]
        [Route("/admin/blog/yazi-guncelle")]
        public IActionResult Update(Guid postId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminBlogPostUpdate");
            var post = _unitOfWork.BlogPost.GetByIdWithCategory(postId);
            var bannerAds = _unitOfWork.BannerAds.Get().ToList();
            foreach (var banner in bannerAds)
            {
                banner.ImageUrl = _storage.BlobUrl + banner.ImageUrl;
            }
            BlogPostUpdateGetVM model = new BlogPostUpdateGetVM
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                FeaturedImageUrl = post.FeaturedImageUrl,
                BlogCategories = _unitOfWork.BlogCategory.Get().ToList(),
                Category = post.Category,
                Tags = _unitOfWork.BlogPost.GetTagsByBlogPostId(postId),
                SummaryContent = post.SummaryContent,
                SeoUrl = post.SeoUrl,
                BannerAds = bannerAds
            };

            return View(model);
        }
        [HttpPost]
        [Route("/admin/blog/yazi-guncelle")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(BlogPostUpdatePostVM data)
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
                var post = _unitOfWork.BlogPost.GetById(data.Id);
                if (!string.IsNullOrEmpty(data.FeaturedImage.Base64Content))
                {
                    var stream = new MemoryStream(_fileManager.ConvertBase64StringToByteArray(data.FeaturedImage.Base64Content));
                    var fileName = data.Title.FormatForTag();
                    var dbPath = await _storage.UploadFile(stream, $"{fileName}.{data.FeaturedImage.Extension}", "blog-featured-images");
                    post.FeaturedImageUrl = dbPath;
                }
                post.Title = data.Title;
                post.SummaryContent = data.SummaryContent;
                post.Content = data.Content;
                post.SeoUrl = data.SeoUrl;
                post.ReadingTime = CalculateReadingTime(data.Content);
                post.CategoryId = data.CategoryId;

                int retVal = _unitOfWork.BlogPost.Update(post, data.Tags);

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

        [HttpGet]
        public IActionResult TogglePostHighLight(Guid postId)
        {
            try
            {
                var post = _unitOfWork.BlogPost.GetById(postId);
                post.IsHighLight = post.IsHighLight == true ? false : true;
                _unitOfWork.BlogPost.Update(post);
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
                    errors = new List<string> { $"Hata: {ex.Message}" }
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
