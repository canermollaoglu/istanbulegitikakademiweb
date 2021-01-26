using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using System;

namespace NitelikliBilisim.App.Components
{
    public class MobileHeaderEducationsMenu : ViewComponent
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        public MobileHeaderEducationsMenu(UnitOfWork userUnitOfWork,IMemoryCache memoryCache)
        {
            _unitOfWork = userUnitOfWork;
            _memoryCache = memoryCache;
        }
        public IViewComponentResult Invoke()
        {
            var menu = _memoryCache.GetOrCreate(CacheKeyUtility.HeaderMenu, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromDays(2);
                return _unitOfWork.Education.GetHeaderEducationMenu();
            });
            return View(menu);
        }
    }
}
