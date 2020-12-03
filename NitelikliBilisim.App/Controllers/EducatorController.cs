using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.App.Filters;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.Main.Educator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Controllers
{
    public class EducatorController : Controller
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

            var model = new GetEducatorDetailVm
            {
                EducatorDetail = _unitOfWork.Educator.GetEducatorDetailUser(educatorId),
                PopularEducations = _unitOfWork.Suggestions.GetGuestUserSuggestedEducations()
            };


            return View(model);
        }
    }
}
