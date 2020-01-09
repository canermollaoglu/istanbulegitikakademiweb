using NitelikliBilisim.Business.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.VmCreator.Suggestion
{
    public class SuggestionVmCreator
    {
        private readonly UnitOfWork _unitOfWork;
        public SuggestionVmCreator(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


    }
}
