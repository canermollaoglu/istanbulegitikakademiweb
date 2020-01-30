using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Support.Enums;
using System.Linq;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.Core.ViewModels.search;
using System.Collections.Generic;

namespace NitelikliBilisim.App.Controllers
{
    public class BrowserController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;

        public BrowserController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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
        public IActionResult GetCourses(string categoryName, string searchText, int page = 0, OrderCriteria order = OrderCriteria.Latest, FiltersVm filter = null)
        {
            var model = new List<EducationVm>();

            model = _unitOfWork.Education.GetInfiniteScrollSearchResults(categoryName, searchText, page, order, filter);

            if (!string.IsNullOrEmpty(categoryName))
                filter.categories = model.Select(x => x.Base.CategoryName).ToArray();

            return Json(new ResponseModel
            {
                data = new
                {
                    model = model,
                    filter = filter
                }
            });
        }
    }
}