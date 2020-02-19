using System;
using System.Security.Claims;
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

        [Route("gruplarim/{ticketId?}")]
        public IActionResult MyGroup(Guid? ticketId)
        {
            if (!ticketId.HasValue)
                return Redirect($"/profil/{User.FindFirstValue(ClaimTypes.NameIdentifier)}");
            var model = _userUnitOfWork.Group.GetMyGroupVm(ticketId.Value);
            if (model == null)
                return Redirect($"/profil/{User.FindFirstValue(ClaimTypes.NameIdentifier)}");
            return View(model);
        }

        [Route("faturalarim")]
        public IActionResult MyInvoices()
        {
            
            return View();
        }

        public IActionResult Cancellation(Guid? ticketId)
        {
            if (!ticketId.HasValue)
                return Redirect($"/profil/{User.FindFirstValue(ClaimTypes.NameIdentifier)}");

            return View();
        }
    }
}
