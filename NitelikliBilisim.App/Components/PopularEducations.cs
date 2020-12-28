using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;
using System.Security.Claims;

namespace NitelikliBilisim.App.Components
{
    public class PopularEducations:ViewComponent
    {
        private readonly UnitOfWork _unitOfWork;
        public PopularEducations(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IViewComponentResult Invoke()
        {
                return View(_unitOfWork.Education.GetPopularEducations(5));
        }
    }
}
