using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
                return View(_unitOfWork.Suggestions.GetUserSuggestedEducations(userId, 5));
        }
    }
}
