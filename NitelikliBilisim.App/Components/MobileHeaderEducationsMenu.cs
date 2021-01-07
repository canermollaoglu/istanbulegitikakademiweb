using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
