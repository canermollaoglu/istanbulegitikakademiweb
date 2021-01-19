using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Components
{
    public class MobileHeaderEducationsMenu : ViewComponent
    {
        private readonly UnitOfWork _unitOfWork;
        public MobileHeaderEducationsMenu(UnitOfWork userUnitOfWork)
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
