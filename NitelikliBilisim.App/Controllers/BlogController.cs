using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.Main.Blog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        [Route("blog/detay/{blogId}")]
        public IActionResult Detail(Guid? blogId)
        {
            if (!blogId.HasValue)
                return RedirectToAction("List");
            try
            {
                var model= _unitOfWork.BlogPost.GetPostById(blogId.Value);
                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        [Route("blog")]
        public IActionResult List(Guid? c,int? p)
        {
            ViewData["CategoryFilter"] = c.HasValue ? c : ViewData["CategoryFilter"];
            ViewData["Page"] = p.HasValue ? p : ViewData["Page"];

            BlogListVm model = new BlogListVm();
            model.Categories = _unitOfWork.BlogCategory.GetListForBlogListPage();
            model.LastBlogPosts = _unitOfWork.BlogPost.LastBlogPosts(5);
            model.Blogs = _unitOfWork.BlogPost.GetPosts(c,p);
            model.TotalBlogPostCount = _unitOfWork.BlogPost.TotalBlogPostCount();
            return View(model);
        }

    }
}
