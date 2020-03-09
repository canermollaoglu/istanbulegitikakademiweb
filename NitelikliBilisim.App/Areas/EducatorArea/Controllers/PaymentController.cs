using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NitelikliBilisim.App.Areas.EducatorArea.Controllers
{
    [Area("EducatorArea"), Authorize("Educator")]
    public class PaymentController : Controller
    {
        [Route("egitmen/odemelerim")]
        public IActionResult MyPayments(Guid? userId)
        {
            if (!userId.HasValue)
                return Redirect("/");

            return View();
        }
    }
}