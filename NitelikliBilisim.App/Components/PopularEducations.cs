using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using System;

namespace NitelikliBilisim.App.Components
{
    public class PopularEducations:ViewComponent
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        public PopularEducations(UnitOfWork unitOfWork,IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
        }

        public IViewComponentResult Invoke()
        {
            var model = _memoryCache.GetOrCreate(CacheKeyUtility.PopularEducations, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromDays(1);
                return _unitOfWork.Education.GetPopularEducations(5);
            });
            return View(model);
        }
    }
}
