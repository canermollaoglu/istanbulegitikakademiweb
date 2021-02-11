using Microsoft.AspNetCore.Mvc;

namespace NitelikliBilisim.App.Controllers
{
    public class ErrorController : Controller
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
