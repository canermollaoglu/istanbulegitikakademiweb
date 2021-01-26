using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NitelikliBilisim.Business.UoW;
using System;

namespace NitelikliBilisim.App.Components
{
    public class HeaderEducationsMenu : ViewComponent
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        public HeaderEducationsMenu(UnitOfWork userUnitOfWork,IMemoryCache memoryCache)
        {
            _unitOfWork = userUnitOfWork;
            _memoryCache = memoryCache;
        }
        public IViewComponentResult Invoke()
        {
            var menu = _memoryCache.GetOrCreate("headereducationsmenu", entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromDays(2);
                return _unitOfWork.Education.GetHeaderEducationMenu();
            });

            return View(menu);
        }
    }
}

