using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;
using System.Security.Claims;

namespace NitelikliBilisim.App.Components
{
    public class PopularEducations:ViewComponent
    {
        private readonly UnitOfWork _unitOfWork;
        public PopularEducations(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IViewComponentResult Invoke()
        {
            var isLoggedIn = HttpContext.User.Identity.IsAuthenticated;
            if (!isLoggedIn)
                return View(_unitOfWork.Suggestions.GetGuestUserSuggestedEducations());
            else
            {
                var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
                return View(_unitOfWork.Suggestions.GetUserSuggestedEducations(userId, 5));
            }
        }
    }
}
