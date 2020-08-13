using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities.blog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.Blog
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class BlogTagController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public BlogTagController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminBlogTagList");
            return View();
        }
        [HttpPost]
        public IActionResult Update(BlogTag data)
        {
            try
            {
                _unitOfWork.BlogTag.Update(data);

                return Json(new ResponseModel
                {
                    isSuccess = true,
                    message = "Tatil günü başarıyla güncellenmiştir."
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata " + ex.Message }
                });
            }

        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _unitOfWork.BlogTag.Delete(id);
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    message = "Etiket başarıyla silinmiştir."
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata " + ex.Message }
                });
            }
        }
        [HttpGet]
        public IActionResult SearchTag(string q)
        {
            try
           {
                var tags = _unitOfWork.BlogTag.Get(x => x.Name.StartsWith(q)).ToList();
                return Json(new ResponseModel
                {
                    isSuccess = true,
                    data =tags
                });
            }
            catch (Exception ex)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata " + ex.Message }
                });
            }
        }
    }
}
