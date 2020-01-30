﻿using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Core.ViewModels.search;
using NitelikliBilisim.Support.Enums;

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
        public async Task<IActionResult> SearchEducationAsync(string searchText, int page = 0, OrderCriteria order = OrderCriteria.Latest, FiltersVm filter = null)
        {
            var model = _unitOfWork.Education.GetInfiniteScrollSearchResults(searchText, page, order, filter);
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