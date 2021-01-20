using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Components
{
    public class HeaderEducationsMenu : ViewComponent
    {
        private readonly UnitOfWork _unitOfWork;
        public HeaderEducationsMenu(UnitOfWork userUnitOfWork)
        {
            _unitOfWork = userUnitOfWork;
        }
        public IViewComponentResult Invoke()
        {
            var menu = _unitOfWork.Education.GetHeaderEducationMenu();
            return View(menu);
        }
    }
}

