using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly UserUnitOfWork _userUnitOfWork;
        public UserProfileController(UserUnitOfWork userUnitOfWork)
        {
            _userUnitOfWork = userUnitOfWork;
        }

        [Route("profil/{userId}")]
        public IActionResult Profile(string userId)
        {
            if (userId == null)
                return Redirect("/");
            var model = _userUnitOfWork.User.GetCustomerInfo(userId);
            return View(model);
        }
    }
}
