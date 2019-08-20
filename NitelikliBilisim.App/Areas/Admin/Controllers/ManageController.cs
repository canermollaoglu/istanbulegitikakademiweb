using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Controllers.Base;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Kategori()
        {
            return View();
        }

        public IActionResult Egitim()
        {
            return View();
        }
    }
}