using Microsoft.AspNetCore.Mvc;

namespace NitelikliBilisim.App.Controllers
{
    public class ErrorController : Controller
    {
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
