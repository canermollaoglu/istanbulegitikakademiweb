
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Areas.Admin.Controllers;
using NitelikliBilisim.App.Models;

namespace NitelikliBilisim.App.Controllers
{
    public class ErrorController : Controller
    {


        //[Authorize]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("error")]
        public IActionResult Error()
        {
            return View("ErrorPage");
        }
        [Route("404")]
        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}
