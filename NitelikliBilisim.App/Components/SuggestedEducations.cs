using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;
using System.Security.Claims;

namespace NitelikliBilisim.App.Components
{
    public class SuggestedEducations : ViewComponent
    {
        private readonly UnitOfWork _unitOfWork;
        public SuggestedEducations(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IViewComponentResult Invoke()
        {
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var data = _unitOfWork.Suggestions.GetUserSuggestedEducations(userId);
            return View(data);
        }
    }
}
