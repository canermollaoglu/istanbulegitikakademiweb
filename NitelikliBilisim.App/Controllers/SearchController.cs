using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels;

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
            var model = new SearchResultsGetVm
            {
                SearchText = searchText
            };
            return View(model);
        }

        [Route("search-for-courses/{searchText}/page/{page}")]
        public IActionResult SearchEducation(string searchText, int page = 0)
        {
            var model = _unitOfWork.Education.GetInfiniteScrollSearchResults(searchText, page);
            return Json(model);
        }
    }
}