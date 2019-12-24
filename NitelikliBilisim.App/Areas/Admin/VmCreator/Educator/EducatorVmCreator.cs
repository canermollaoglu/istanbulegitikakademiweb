using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.areas.admin.educator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.VmCreator.Educator
{
    public class EducatorVmCreator
    {
        private readonly UnitOfWork _unitOfWork;
        public EducatorVmCreator(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<_Educator> GetEducators()
        {
            return _unitOfWork.Educator.GetEducators();
        }
    }
}
