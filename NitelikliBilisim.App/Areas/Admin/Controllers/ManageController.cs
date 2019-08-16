using Microsoft.AspNetCore.Mvc;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    public class ManageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Kategori()
        {
            return View();
        }
    }
}