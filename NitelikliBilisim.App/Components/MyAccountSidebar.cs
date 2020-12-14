using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Components
{
    public class MyAccountSidebar : ViewComponent
    {
        private readonly UserUnitOfWork _userUnitOfWork;
        public MyAccountSidebar(UserUnitOfWork userUnitOfWork)
        {
            _userUnitOfWork = userUnitOfWork;
        }
        public IViewComponentResult Invoke(string currentPageName)
        {
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(_userUnitOfWork.User.GetMyAccountSidebarInfo(userId, currentPageName));
        }
    }
}
