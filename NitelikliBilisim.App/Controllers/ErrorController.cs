using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Controllers.Base;

namespace NitelikliBilisim.App.Controllers
{
    public class ErrorController : BaseController
    {
        
        [Route("Error/{statusCode}")]
        public IActionResult ErrorUI(int statusCode)
        {
            if (statusCode == 404)
            {
                return RedirectToAction(nameof(PageNotFound));
            }
            return View();
        }
        [Route("sayfa-bulunamadi")]
        public IActionResult PageNotFound() {

            return View();
        }
    }
}
