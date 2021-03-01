using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Entities.blog;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.Blog
{
    public class BlogCategoryController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMemoryCache _memCache;

        public BlogCategoryController(UnitOfWork unitOfWork, IMemoryCache memCache)
        {
            _unitOfWork = unitOfWork;
            _memCache = memCache;
        }
        [Route("/admin/blog/kategori-listesi")]
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminBlogCategoryList");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(BlogCategory data)
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

            _unitOfWork.BlogCategory.Insert(data);
            RefreshCache();
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Blog kategorisi başarı ile eklenmiştir."
            });



        }

        public IActionResult Delete(Guid? categoryId)
        {
            if (!categoryId.HasValue)
            {
                return Json(new ResponseModel
                {
                    isSuccess = false,
                    errors = new List<string> { "Hata: Sayfayı yenileyerek tekrar deneyiniz." }
                });
            }
            _unitOfWork.BlogCategory.Delete(categoryId.Value);
            RefreshCache();
            return Json(new ResponseModel
            {
                isSuccess = true,
                message = "Kategori başarıyla silinmiştir."
            });


        }

        [HttpGet]
        [Route("/admin/blog/kategori-guncelle")]
        public IActionResult Update(Guid? categoryId)
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminBlogCategoryUpdate");
            if (categoryId == null)
                return Redirect("/admin/blog/kategori-listesi");
            var category = _unitOfWork.BlogCategory.GetById(categoryId.Value);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(BlogCategory data)
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
            _unitOfWork.BlogCategory.Update(data);
            RefreshCache();
            return Json(new ResponseModel
            {
                isSuccess = true
            });

        }
        private void RefreshCache()
        {
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(1)
            };
            _memCache.Remove(CacheKeyUtility.BlogCategories);
            _memCache.Set(CacheKeyUtility.BlogCategories, _unitOfWork.BlogCategory.GetListForBlogListPage(), options);
        }

    }
}
