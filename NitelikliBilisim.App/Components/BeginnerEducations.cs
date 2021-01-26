using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NitelikliBilisim.Business.UoW;
using System;

namespace NitelikliBilisim.App.Components
{
    public class BeginnerEducations : ViewComponent
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        public BeginnerEducations(UnitOfWork unitOfWork,IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
        }

        public IViewComponentResult Invoke()
        {
            var model = _memoryCache.GetOrCreate("beginnerleveleducations", entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromDays(2);
                return _unitOfWork.Education.GetBeginnerEducations(5);
            });


            //_unitOfWork.Education.GetBeginnerEducations(5);
            return View(model);

        }
    }
}
