using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.VmCreator.EducationParts
{
    public class EducationPartVmCreator
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationPartVmCreator(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ManageVm CreateManageVm(Guid educationId)
        {
            var education = _unitOfWork.Education.GetById(educationId);
            return new ManageVm
            {
                EducationId = education.Id,
                EducationName = education.Name
            };
        }

        public GetEducationPartsVm CreateEducationPartsVm(Guid educationId)
        {
            var parts = _unitOfWork.EducationPart.Get(x => x.EducationId == educationId, x => x.OrderBy(o => o.Order));

            var educationParts = new List<_EducationPart>();
            foreach (var item in parts)
                educationParts.Add(new _EducationPart
                {
                    Id = item.Id,
                    EducationId = item.EducationId,
                    Title = item.Title,
                    Order = item.Order,
                    Duration = item.Duration
                });

            return new GetEducationPartsVm
            {
                EducationParts = educationParts
            };
        }
    }
}
