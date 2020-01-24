using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services;
using NitelikliBilisim.Core.ViewModels;
using NitelikliBilisim.Support.Enums;
using System.Linq;

namespace NitelikliBilisim.App.Controllers
{
    public class BrowserController : Controller
    {
        private readonly UnitOfWork _unitOfWork;

        public BrowserController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Route("tum-egitimler/{categoryUrl?}")]
        public IActionResult AllCourses(string categoryUrl, string showAs = "grid")
        {
            var categoryNames = _unitOfWork.EducationCategory.Get().Select(x => x.Name).ToList();

            var category = categoryNames.FirstOrDefault(x=> StringHelper.UrlFormatConverter(x) == categoryUrl) ?? "";

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
        public IActionResult GetAllCourses(string category, int page = 0, OrderCriteria order = OrderCriteria.Latest)
        {
            var model = _unitOfWork.Education.GetEducationsByCategory(category, page, order);
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