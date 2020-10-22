using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.Applications
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class EducatorApplicationController : Controller
    {
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducatorApplicationList");
            return View();
        }
    }
}
