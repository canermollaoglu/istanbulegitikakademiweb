using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Controllers.Base;
using NitelikliBilisim.App.Filters;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Controllers
{
    public class EducatorController : BaseController
    {
        private readonly UnitOfWork _unitOfWork;
        public EducatorController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [TypeFilter(typeof(UserLoggerFilterAttribute))]
        [Route("egitmen-detayi/{educatorId}")]
        public IActionResult Details(string educatorId)
        {
            if (string.IsNullOrEmpty(educatorId))
                return Redirect("/");

            var model = _unitOfWork.Educator.GetEducatorDetailUser(educatorId);
            return View(model);
        }
    }
}
