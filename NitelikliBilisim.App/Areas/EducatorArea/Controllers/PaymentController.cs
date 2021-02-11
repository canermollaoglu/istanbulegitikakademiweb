using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.EducatorArea.Controllers
{
    [Area("EducatorArea"), Authorize(Roles = "Educator")]
    public class PaymentController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public PaymentController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Route("egitmen/odemelerim")]
        public IActionResult MyPayments()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (userId == null)
                return Redirect("/");
            var model = _unitOfWork.Educator.GetMySalaries(userId);
            return View(model);
        }
    }
}