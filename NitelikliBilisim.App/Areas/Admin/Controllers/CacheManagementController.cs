using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{

    public class CacheManagementController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMemoryCache _memCache;
        public CacheManagementController(IMemoryCache memCache, UnitOfWork unitOfWork)
        {
            _memCache = memCache;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Management()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminCacheManagement");
            var keys = CacheKeyUtility.GetAllCacheKeys();
            Dictionary<string, bool> caches = new();
            foreach (var key in keys)
            {
                caches.Add(key, _memCache.Get(key) != null);
            }

            return View(caches);
        }

        [HttpPost]
        public IActionResult RemoveCache(string data)
        {

            if (data != null)
            {
                _memCache.Remove(data);
            }
            else
            {
                var keys = CacheKeyUtility.GetAllCacheKeys();
                foreach (var key in keys)
                {
                    _memCache.Remove(key);
                }
            }

            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }

        [HttpPost]
        public IActionResult AddCache(string data)
        {

            SetCache(data);

            return Json(new ResponseModel
            {
                isSuccess = true
            });
        }

        private void SetCache(string key)
        {
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(1)
            };
            if (string.IsNullOrEmpty(key))
            {
                _memCache.Set(CacheKeyUtility.HeaderMenu, _unitOfWork.Education.GetHeaderEducationMenu(), options);
                _memCache.Set(CacheKeyUtility.HomeNbuyCategories, _unitOfWork.EducationCategory.GetNBUYEducationCategories(), options);
                _memCache.Set(CacheKeyUtility.HomeUserComments, _unitOfWork.EducationComment.GetHighlightComments(5), options);
                _memCache.Set(CacheKeyUtility.BeginnerEducations, _unitOfWork.Education.GetBeginnerEducations(5), options);
                _memCache.Set(CacheKeyUtility.PopularEducations, _unitOfWork.Education.GetPopularEducations(5), options);
                _memCache.Set(CacheKeyUtility.HomeEducationTags, _unitOfWork.Education.GetEducationSearchTags(), options);
                _memCache.Set(CacheKeyUtility.BlogCategories, _unitOfWork.BlogCategory.GetListForBlogListPage(), options);
                _memCache.Set(CacheKeyUtility.BlogLastPosts, _unitOfWork.BlogPost.LastBlogPosts(5), options);
            }
            else if (key.Equals(CacheKeyUtility.BeginnerEducations))
                _memCache.Set(key, _unitOfWork.Education.GetBeginnerEducations(5), options);
            else if (key.Equals(CacheKeyUtility.HeaderMenu))
                _memCache.Set(key, _unitOfWork.Education.GetHeaderEducationMenu(), options);
            else if (key.Equals(CacheKeyUtility.HomeNbuyCategories))
                _memCache.Set(key, _unitOfWork.EducationCategory.GetNBUYEducationCategories(), options);
            else if (key.Equals(CacheKeyUtility.HomeUserComments))
                _memCache.Set(key, _unitOfWork.EducationComment.GetHighlightComments(5), options);
            else if (key.Equals(CacheKeyUtility.PopularEducations))
                _memCache.Set(key, _unitOfWork.Education.GetPopularEducations(5), options);
            else if (key.Equals(CacheKeyUtility.HomeEducationTags))
                _memCache.Set(key, _unitOfWork.Education.GetEducationSearchTags(), options);
            else if (key.Equals(CacheKeyUtility.BlogCategories))
                _memCache.Set(key, _unitOfWork.BlogCategory.GetListForBlogListPage(), options);
            else if (key.Equals(CacheKeyUtility.BlogLastPosts))
                _memCache.Set(key, _unitOfWork.BlogPost.LastBlogPosts(5), options);

        }

    }
}
