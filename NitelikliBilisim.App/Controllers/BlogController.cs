using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Filters;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.Main.Blog;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace NitelikliBilisim.App.Controllers
{
    public class BlogController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;
        private readonly IMemoryCache _memoryCache;
        public BlogController(UnitOfWork unitOfWork,IStorageService storageService,IMemoryCache memoryCache)
        {
            _storageService = storageService;
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            return View();
        }

        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [Route("blog/{catSeoUrl}/{seoUrl}")]
        public IActionResult Detail(string catSeoUrl, string seoUrl)
        {
            if (string.IsNullOrEmpty(seoUrl) || string.IsNullOrEmpty(catSeoUrl))
                return Redirect("/");
            var checkBlogPost = _unitOfWork.BlogPost.CheckBlogBySeoUrl(seoUrl);
            if (!checkBlogPost)
                return RedirectToAction("PageNotFound", "Error");

            try
            {
                var model = _unitOfWork.BlogPost.GetPostBySeoUrl(seoUrl);
                if (model.Content.Contains("[##") && model.Content.Contains("##]"))
                {
                    var index = model.Content.IndexOf("[##");
                    var lastChar = model.Content.IndexOf("##]", index);

                    var code = model.Content.Substring(index + 3, lastChar - index - 3);
                    var banner =_unitOfWork.BannerAds.GetBannerByCode(code);
                    if (banner != null)
                    {
                        var htmlContent = $"<a target=\"_blank\" href=\"{banner.RelatedApplicationUrl}\" class=\"custom-banner white\">";
                        htmlContent += "<div class=\"custom-banner__icon\">";
                        htmlContent += "<span class=\"icon-outer\">";
                        htmlContent += "<svg class=\"icon\">";
                        htmlContent += $"<use xlink:href=\"../../assets/img/icons.svg#{banner.IconUrl}\"></use>";
                        htmlContent += "</svg>";
                        htmlContent += "</span></div>";
                        htmlContent += "<div class=\"custom-banner__cnt\">";
                        htmlContent += $"<div class=\"custom-banner__title\">{banner.Title1} <span class=\"title--blue\">{banner.Title2}</span></div>";
                        htmlContent += $"<div class=\"custom-banner__txt\">{banner.Content}</div>";
                        htmlContent += "</div>";
                        htmlContent += "<div class=\"custom-banner__img\">";
                        htmlContent += $"<img src=\"{_storageService.BlobUrl + banner.ImageUrl}\">";
                        htmlContent += "</div></a>";
                        model.Content = model.Content.Replace("[##" + code + "##]", htmlContent);
                    }
                }

                Regex ItemRegex = new Regex(@"<h[2][^>]*?>(?<TagText>.*?)</h[2]>", RegexOptions.Compiled);
                foreach (Match ItemMatch in ItemRegex.Matches(model.Content))
                {
                    var startIndex = ItemMatch.Value.IndexOf(">");
                    var endIndex = ItemMatch.Value.LastIndexOf("<");

                    var title = ItemMatch.Value.Substring(startIndex+1, (endIndex - startIndex)-1);
                    var array = title.Split(" ");
                    if (array.Length>1)
                    {
                        var firstHalf = "";
                        var secondHalf = "";
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (i<array.Length/2)
                                firstHalf += array[i]+" ";
                            else
                                secondHalf += array[i]+" ";
                        }
                        secondHalf = $"<span class='title--blue'>{secondHalf}</span>";
                        title = (firstHalf + secondHalf).Trim();
                    }


                    model.Content = model.Content.Replace(ItemMatch.Value, $"<h2 class='blog-detail__title'>{title}</h2>");
                }

                return View(model);
            }
            catch 
            {
                return RedirectToAction("PageNotFound", "Error");
            }

        }

        [Route("blog/{c?}")]
        public IActionResult List(string c,string sKey)
        {
            BlogListVm model = new BlogListVm();
            model.CurrentCategorySeoUrl = c;
            model.SearchKey = sKey;
            var categories = _memoryCache.GetOrCreate(CacheKeyUtility.BlogCategories, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromDays(1);
                return _unitOfWork.BlogCategory.GetListForBlogListPage();
            });
            var currentC = categories.FirstOrDefault(x => x.SeoUrl == c);
            
            model.CurrentCategory = currentC != null ? currentC.Name:null;


            model.Categories = categories;

            var lastPost = _memoryCache.GetOrCreate(CacheKeyUtility.BlogLastPosts, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromDays(1);
                return _unitOfWork.BlogPost.LastBlogPosts(5);
            });
            foreach (var post in lastPost)
            {
                post.ViewCount = _unitOfWork.BlogPost.GetBlogPostViewCount(post.SeoUrl, post.CategorySeoUrl);
            }

            model.LastBlogPosts = lastPost;
            model.TotalBlogPostCount = _unitOfWork.BlogPost.TotalBlogPostCount();
            return View(model);
        }

        [HttpPost]
        [Route("get-blog-posts")]
        public IActionResult GetBlogPosts(string categoryId, string searchKey, int page = 1)
        {
            var model = _unitOfWork.BlogPost.GetPosts(categoryId, page, searchKey);

            foreach (var post in model.Posts)
            {
                post.FeaturedImageUrl = _storageService.BlobUrl + post.FeaturedImageUrl;
            }
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = model
            });

        }

    }
}
