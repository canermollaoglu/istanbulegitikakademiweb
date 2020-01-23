using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Models;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
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

        [Route("tum-egitimler/{categoryName?}")]
        public IActionResult AllCourses(string categoryName, string showAs = "grid")
        {
            var category = _unitOfWork.EducationCategory.Get(x => x.Name.ToLower() == categoryName).FirstOrDefault();

            if (category == null)
                categoryName = "";

            var model = new SearchResultsGetVm
            {
                SearchText = categoryName,
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