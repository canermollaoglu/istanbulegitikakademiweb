using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NitelikliBilisim.App.Controllers
{
    public class SaleController : Controller
    {
        [Route("sepet")]
        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Payment()
        {
            return View();
        }
    }
}