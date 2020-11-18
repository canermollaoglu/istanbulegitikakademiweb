
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;

namespace NitelikliBilisim.App.Controllers
{
    public class ErrorController : Controller
    {

        //[Authorize]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("error")]
        public IActionResult Error(int eCode)
        {
            if (eCode == StatusCodes.Status404NotFound)
            {
                return RedirectToAction("PageNotFound");
            }

            return View("ErrorPage");
            
        }
        [Route("error/404")]
        public IActionResult PageNotFound()
        {

            return View();
        }
    }
}
