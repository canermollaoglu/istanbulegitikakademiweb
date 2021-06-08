using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using System;
using System.Security.Claims;

namespace NitelikliBilisim.App.Components
{
    public class PopularEducations : ViewComponent
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMemoryCache _memoryCache;
        public PopularEducations(UnitOfWork unitOfWork, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
        }

        public IViewComponentResult Invoke()
        {
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return View(_unitOfWork.Education.GetGuestUserPopularEducations());
            }
            else
            {
                return View(_unitOfWork.Education.GetCustomerUserPopularEducations(userId));
            }
        }
    }
}
