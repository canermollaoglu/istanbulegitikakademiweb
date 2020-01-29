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

        [Route("tum-egitimler/{categoryUrl?}")]
        public IActionResult AllCourses(string categoryUrl, string showAs = "grid")
        {
            var categoryNames = _unitOfWork.EducationCategory.Get().Select(x => x.Name).ToList();

            var category = categoryNames.FirstOrDefault(x => StringHelper.UrlFormatConverter(x) == categoryUrl) ?? "";

            var model = new SearchResultsGetVm
            {
                SearchText = category,
                OrderCriterias = EnumSupport.ToKeyValuePair<OrderCriteria>(),
                ShowAs = showAs
            };
            return View(model);
        }

        [HttpPost]
        [Route("get-all-courses")]
        public async Task<IActionResult> GetAllCoursesAsync(string category, int page = 0, OrderCriteria order = OrderCriteria.Latest)
        {
            var model = _unitOfWork.Education.GetEducationsByCategory(category, page, order);
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
    }
}