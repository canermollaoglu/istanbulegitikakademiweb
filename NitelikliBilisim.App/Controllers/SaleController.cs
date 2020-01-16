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

        [HttpPost, IgnoreAntiforgeryToken, Route("get-cart-items")]
        public IActionResult GetCartItems(GetCartItemsData data)
        {

            return Json("");
        }

        public IActionResult Payment()
        {
            return View();
        }
    }

    public class GetCartItemsData
    {
        public List<Guid> Items { get; set; }
    }
}