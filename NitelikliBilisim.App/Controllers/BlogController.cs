using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Filters;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.Main.Blog;
using System;
using System.Linq;

namespace NitelikliBilisim.App.Controllers
{
    public class BlogController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;
        public BlogController(UnitOfWork unitOfWork,IStorageService storageService)
        {
            _storageService = storageService;
            _unitOfWork = unitOfWork;
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
                return NotFound();

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

                return View(model);
            }
            catch (Exception)
            {

                throw;
            }

        }


        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [Route("blog/{c?}")]
        public IActionResult List(string c, int? p)
        {
            ViewData["CategoryFilter"] = !string.IsNullOrEmpty(c) ? c : ViewData["CategoryFilter"];
            ViewData["Page"] = p.HasValue ? p : ViewData["Page"];

            BlogListVm model = new BlogListVm();
            var categories = _unitOfWork.BlogCategory.GetListForBlogListPage();
            var currentC = categories.FirstOrDefault(x => x.SeoUrl == c);
            model.Categories = categories;
            model.CurrentCategory = currentC != null ? currentC.Name:null;
            model.LastBlogPosts = _unitOfWork.BlogPost.LastBlogPosts(5);
            model.Blogs = _unitOfWork.BlogPost.GetPosts(c, p);
            model.TotalBlogPostCount = _unitOfWork.BlogPost.TotalBlogPostCount();
            return View(model);
        }

    }
}
