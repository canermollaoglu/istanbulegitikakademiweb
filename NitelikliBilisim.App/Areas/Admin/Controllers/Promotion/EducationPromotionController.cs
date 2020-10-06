using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Lexicographer;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Areas.Admin.Controllers.Promotion
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class EducationPromotionController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationPromotionController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Route("admin/promosyonlar")]
        public IActionResult List()
        {
            ViewData["bread_crumbs"] = BreadCrumbDictionary.ReadPart("AdminEducationPromotionList");
            return View();
        }
    }
}
