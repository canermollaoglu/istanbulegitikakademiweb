using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NitelikliBilisim.App.Utility;
using NitelikliBilisim.Business.UoW;
using System;
using System.Security.Claims;

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
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return View(_unitOfWork.Education.GetGuestUserBeginnerEducations(3));
            }
            else
            {
                return View(_unitOfWork.Education.GetCustomerUserBeginnerEducations(3, userId));
            }

        }
    }
}
