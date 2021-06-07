using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Services.Abstracts;

namespace NitelikliBilisim.App.Components
{
    public class HeaderLoggedInUserDropDown : ViewComponent
    {
        private readonly UserUnitOfWork _unitOfWork;
        public HeaderLoggedInUserDropDown(UserUnitOfWork userUnitOfWork, IStorageService storageService)
        {
            _unitOfWork = userUnitOfWork;
        }
        public IViewComponentResult Invoke()
        {
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggenInUserInfo = _unitOfWork.User.GetHeaderLoggedInUserDropDownInfo(userId);
            return View(loggenInUserInfo);
        }
    }
}
