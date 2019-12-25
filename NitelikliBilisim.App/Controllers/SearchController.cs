using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Controllers
{
    public class SearchController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public SearchController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("arama-sonuclari/{searchText}")]
        public IActionResult SearchResults(string searchText)
        {
            return View();
        }
        public IActionResult SearchEducation(string searchText, int page = 0)
        {
            var model = _unitOfWork.Education.GetInfiniteScrollSearchResults(searchText, page);
            return Json(model);
        }
    }
}