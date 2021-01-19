using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;

namespace NitelikliBilisim.App.Components
{
    public class BeginnerEducations : ViewComponent
    {
        private readonly UnitOfWork _unitOfWork;
        public BeginnerEducations(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IViewComponentResult Invoke()
        {
            var model = _unitOfWork.Education.GetBeginnerEducations(5);
            return View(model);

        }
    }
}
