using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.search;
using NitelikliBilisim.Enums;
using System.Linq;

namespace NitelikliBilisim.App.Controllers
{
    //[Authorize]
    public class SearchController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        public SearchController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("arama-sonuclari/{searchText}")]
        public IActionResult SearchResults(string searchText, string showAs = "grid")
        {
            var model = new SearchResultsGetVm
            {
                SearchText = searchText,
                OrderCriterias = EnumSupport.ToKeyValuePair<OrderCriteria>(),
                ShowAs = showAs
            };
            return View(model);
        }

        //[Route("search-for-courses/{searchText}/page/{page}")]
        //public IActionResult SearchEducation(string searchText, int page = 0, FilterOptionsVm filter = null)
        //{
        //    var model = _unitOfWork.Education.GetInfiniteScrollSearchResults(searchText, page, filter);
        //    return Json(new ResponseModel
        //    {
        //        data = new
        //        {
        //            model = model
        //        }
        //    });
        //}

        [HttpPost]
        [Route("search-for-courses")]
        public IActionResult SearchEducation(string searchText, int page = 0, FilterOptionsVm filter = null)
        {
            var model = _unitOfWork.Education.GetInfiniteScrollSearchResults(searchText, page, filter);
            return Json(new ResponseModel
            {
                data = new
                {
                    model = model
                }
            });
        }

        [HttpPost]
        [Route("get-searched-categories")]
        public IActionResult GetSearchedCategories(string searchText, string[] chosen)
        {
            var model = _unitOfWork.EducationCategory.GetSearchedEducationCategories(searchText);

            foreach (var item in model)
            {
                if (chosen.Contains(item.name))
                    item.isChecked = true;
            }

            return Json(new ResponseModel
            {
                data = model
            });
        }
    }
}