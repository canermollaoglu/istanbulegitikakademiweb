using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;
using System;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class BlogPostController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public BlogPostController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult List()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult Add()
        //{

        //}

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
            return Json("a");
        }
    }
}
