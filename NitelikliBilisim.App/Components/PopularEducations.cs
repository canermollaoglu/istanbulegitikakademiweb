using Microsoft.AspNetCore.Mvc;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return View(_unitOfWork.Suggestions.GetGuestUserSuggestedEducations());
        }



    }
}
