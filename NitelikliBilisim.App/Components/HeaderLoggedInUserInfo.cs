using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Services.Abstracts;
using System.Security.Claims;


namespace NitelikliBilisim.App.Components
{
    public class HeaderLoggedInUserInfo : ViewComponent
    {
        private readonly UserUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;
        public HeaderLoggedInUserInfo(UserUnitOfWork userUnitOfWork,IStorageService storageService)
        {
            _unitOfWork = userUnitOfWork;
            _storageService = storageService;
        }
        public IViewComponentResult Invoke()
        {
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggenInUserInfo = _unitOfWork.User.GetHeaderLoggedInUserInfo(userId);
            if (!string.IsNullOrEmpty(loggenInUserInfo.AvatarPath))
            {
                loggenInUserInfo.AvatarPath = _storageService.BlobUrl + loggenInUserInfo.AvatarPath;
            }

            return View(loggenInUserInfo);
        }
    }
}

