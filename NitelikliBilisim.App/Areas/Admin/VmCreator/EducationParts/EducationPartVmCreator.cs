using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_parts;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var baseParts = _unitOfWork.EducationPart.Get(x => x.EducationId == educationId && x.BasePartId == null)
                .Select(x => new _EducationPart
                {
                    Id = x.Id,
                    EducationId = x.EducationId,
                    Title = x.Title
                }).ToList();
            return new ManageVm
            {
                EducationId = education.Id,
                EducationName = education.Name,
                BaseParts = baseParts
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
                    Duration = item.Duration,
                    BasePartId = item.BasePartId,
                    BasePartTitle = item.BasePartId != null ? parts.First(x => x.Id == item.BasePartId).Title : "Üst Başlık"
                });

            return new GetEducationPartsVm
            {
                EducationParts = educationParts
            };
        }

        public List<_EducationPart> CreateBaseParts(Guid educationId)
        {
            var parts = _unitOfWork.EducationPart.Get(x => x.EducationId == educationId && x.BasePartId == null, x => x.OrderBy(o => o.Order));

            var vm = new List<_EducationPart>();
            foreach (var item in parts)
                vm.Add(new _EducationPart
                {
                    Id = item.Id,
                    EducationId = item.EducationId,
                    Title = item.Title,
                    Order = item.Order
                });

            return vm;
        }
    }
}
