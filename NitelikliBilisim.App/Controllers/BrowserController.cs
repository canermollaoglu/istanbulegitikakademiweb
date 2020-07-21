using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Support.Enums;
using System.Linq;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.Core.Services.Abstracts;
using System.IO;
using System.Threading.Tasks;
using NitelikliBilisim.Core.ViewModels.search;
using NitelikliBilisim.App.Filters;

namespace NitelikliBilisim.App.Controllers
{
    public class BrowserController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;

        public BrowserController(UnitOfWork unitOfWork, IStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
        }
        [UserLoggerFilter]
        [Route("egitimler/{categoryUrl?}")]
        public IActionResult Courses(string categoryUrl, string s, string showAs = "grid")
        {
            var categoryNames = _unitOfWork.EducationCategory.Get(x => x.IsCurrent && x.BaseCategoryId == null).Select(x => x.Name).ToList();
            var categoryName = categoryNames.FirstOrDefault(x => StringHelper.UrlFormatConverter(x) == categoryUrl) ?? "";

            var model = new SearchResultsGetVm
            {
                CategoryName = categoryName,
                SearchText = s,
                OrderCriterias = EnumSupport.ToKeyValuePair<OrderCriteria>(),
                ShowAs = showAs
            };
            return View(model);
        }

        [HttpPost]
        [Route("get-courses")]
        public async Task<IActionResult> GetCourses(string categoryName, string searchText, int page = 0, OrderCriteria order = OrderCriteria.Latest, FiltersVm filter = null)
        {
            var model = _unitOfWork.Education.GetInfiniteScrollSearchResults(categoryName, searchText, page, order, filter);

            foreach (var item in model)
            {
                for (int i = 0; i < item.Medias.Count; i++)
                {
                    var folder = Path.GetDirectoryName(item.Medias[i].FileUrl);
                    var fileName = Path.GetFileName(item.Medias[i].FileUrl);
                    item.Medias[i].FileUrl = await _storageService.DownloadFile(fileName, folder);
                }
            }

            var filterOptions = _unitOfWork.Education.GetEducationFilterOptions(categoryName, searchText, filter);

            return Json(new ResponseModel
            {
                data = new
                {
                    model = model,
                    filterOptions = filterOptions
                }
            });
        }
    }
}