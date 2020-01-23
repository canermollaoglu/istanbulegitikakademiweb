using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_gains;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.App.Areas.Admin.VmCreator.EducationGains
{
    public class EducationGainVmCreator
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationGainVmCreator(UnitOfWork unitOfWork)
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

        public GetEducationGainsVm CreateEducationGainsVm(Guid educationId)
        {
            var gains = _unitOfWork.EducationGain.Get(x => x.EducationId == educationId, x => x.OrderBy(o => o.CreatedDate));

            var educationGains = new List<_EducationGain>();
            foreach (var item in gains)
                educationGains.Add(new _EducationGain
                {
                    Id = item.Id,
                    EducationId = item.EducationId,
                    Gain = item.Gain
                });

            return new GetEducationGainsVm
            {
                EducationGains = educationGains
            };
        }
    }
}
