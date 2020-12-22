using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.Main.Blog;
using System;

namespace NitelikliBilisim.App.Controllers
{
    public class BlogController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        public BlogController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
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
                return View(model);
            }
            catch (Exception)
            {

                throw;
            }

        }
        [Route("blog/{c?}")]
        public IActionResult List(string c, int? p)
        {
            ViewData["CategoryFilter"] = !string.IsNullOrEmpty(c)? c : ViewData["CategoryFilter"];
            ViewData["Page"] = p.HasValue ? p : ViewData["Page"];

            BlogListVm model = new BlogListVm();
            model.Categories = _unitOfWork.BlogCategory.GetListForBlogListPage();
            model.LastBlogPosts = _unitOfWork.BlogPost.LastBlogPosts(5);
            model.Blogs = _unitOfWork.BlogPost.GetPosts(c, p);
            model.TotalBlogPostCount = _unitOfWork.BlogPost.TotalBlogPostCount();
            return View(model);
        }

    }
}
