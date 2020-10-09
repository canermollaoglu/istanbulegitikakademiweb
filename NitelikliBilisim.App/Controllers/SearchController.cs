using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MUsefulMethods;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Filters;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.search;

namespace NitelikliBilisim.App.Controllers
{
    //[Authorize]
    public class SearchController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;
        public SearchController(UnitOfWork unitOfWork, IStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
        }
        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [Route("arama-sonuclari/{searchText}")]
        public IActionResult SearchResults(string searchText, string showAs = "grid")
        {
            var model = new SearchResultsGetVm
            {
                SearchText = searchText,
                OrderCriterias = EnumHelpers.ToKeyValuePair<OrderCriteria>(),
                ShowAs = showAs
            };
            return View(model);
        }
        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [HttpPost]
        [Route("search-for-courses")]
        public async Task<IActionResult> SearchEducation(string searchText, int page = 0, OrderCriteria order = OrderCriteria.Latest, FiltersVm filter = null)
        {
            var model = _unitOfWork.Education.GetInfiniteScrollSearchResults("", searchText, page, order, filter);
            foreach (var item in model)
            {
                for (int i = 0; i < item.Medias.Count; i++)
                {
                    var folder = Path.GetDirectoryName(item.Medias[i].FileUrl);
                    var fileName = Path.GetFileName(item.Medias[i].FileUrl);
                    item.Medias[i].FileUrl = await _storageService.DownloadFile(fileName, folder);
                }
            }
            return Json(new ResponseModel
            {
                data = new
                {
                    model = model
                }
            });
        }

        [HttpPost]
        [Route("get-filter-options")]
        public IActionResult GetFilterOptions(string categoryName, string searchText)
        {
            var model = _unitOfWork.Education.GetEducationFilterOptions(categoryName, searchText);

            return Json(new ResponseModel
            {
                data = model
            });
        }
    }
}