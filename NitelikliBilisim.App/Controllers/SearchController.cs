﻿using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.search;
using NitelikliBilisim.Support.Enums;

namespace NitelikliBilisim.App.Controllers
{
    //[Authorize]
    public class SearchController : BaseController
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

        [HttpPost]
        [Route("search-for-courses")]
        public IActionResult SearchEducation(string searchText, int page = 0, OrderCriteria order = OrderCriteria.Latest, FiltersVm filter = null)
        {
            var model = _unitOfWork.Education.GetInfiniteScrollSearchResults("", searchText, page, order, filter);
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
        public IActionResult GetFilterOptions(string searchText)
        {
            var model = _unitOfWork.Education.GetEducationFilterOptions(searchText);

            return Json(new ResponseModel
            {
                data = model
            });
        }
    }
}