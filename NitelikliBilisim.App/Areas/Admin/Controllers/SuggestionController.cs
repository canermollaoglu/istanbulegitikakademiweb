using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.areas.admin.suggestion;

namespace NitelikliBilisim.App.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SuggestionController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public SuggestionController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("admin/oneri-kategori-yonetimi")]
        public IActionResult Manage()
        {
            var categories = _unitOfWork.EducationCategory.Get(x => x.BaseCategoryId == null && x.CategoryType == CategoryType.NBUY, x => x.OrderBy(o => o.Name));

            var model = new ManageGetVm
            {
                Categories = categories
            };
            return View(model);
        }
        [Route("admin/get-suggestions")]
        public IActionResult GetSuggestions()
        {
            var model = _unitOfWork.Suggestion.Get(null, x => x.OrderBy(o => o.RangeMin));
            return Json(new ResponseModel
            {
                isSuccess = true,
                data = new
                {
                    suggestions = model
                }
            });
        }

        [Route("admin/add-suggestion")]
        public IActionResult AddSuggestion()
        {

            return Json("");
        }

        [Route("admin/delete-suggestion/{suggestionId}")]
        public IActionResult DeleteSuggestion(Guid? suggestionId)
        {

            return Json("");
        }
    }
}