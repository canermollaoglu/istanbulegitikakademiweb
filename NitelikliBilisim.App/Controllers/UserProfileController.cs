using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NitelikliBilisim.App.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        [Route("profil/{userId}")]
        public IActionResult Profile(string userId)
        {
            if (userId == null)
                return Redirect("/");

            return View();
        }
    }
}
