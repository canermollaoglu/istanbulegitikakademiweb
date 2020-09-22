using NitelikliBilisim.App.Areas.Admin.Models.Education;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.DTO;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.areas.admin.education;
using NitelikliBilisim.Support.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NitelikliBilisim.App.Areas.Admin.VmCreator.Education
{
    public class EducationVmCreator
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationVmCreator(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public AddGetVm CreateAddGetVm()
        {
            var tags = _unitOfWork.EducationTag.Get(null, x => x.OrderBy(o => o.Name));
            var categories = _unitOfWork.EducationCategory.Get(x => x.BaseCategoryId != null, x => x.OrderBy(o => o.Name));
            var levels = EnumSupport.ToKeyValuePair<EducationLevel>();
            return new AddGetVm
            {
                Levels = levels,
                Categories = categories
            };
        }

        public List<EducationDto> CreateListGetVm(int page = 0, int shownRecords = 15)
        {
            return _unitOfWork.Education.GetPagedEducations(page, shownRecords);
        }

        public UpdateGetVm CreateUpdateGetVm(Guid educationId)
        {
            var addGetVm = CreateAddGetVm();
            var education = _unitOfWork.Education.GetById(educationId);
            var relatedTags = _unitOfWork.Education.GetTags(educationId);
            return new UpdateGetVm
            {
                Levels = addGetVm.Levels,
                Education = education,
                RelatedTags = relatedTags
            };
        }

        public void SendVmToUpdate(UpdatePostVm data)
        {
            _unitOfWork.Education.Update(new Core.Entities.Education
            {
                Id = data.EducationId,
                CategoryId = data.CategoryId,
                Days = data.Days.GetValueOrDefault(),
                HoursPerDay = data.HoursPerDay.GetValueOrDefault(),
                Description = data.Description,
                Description2 = data.Description2,
                Level = (EducationLevel)data.EducationLevel,
                Name = data.Name,
                IsActive = data.IsActive
            }, data.Tags);
        }

        public Core.ViewModels.areas.admin.educator.ManageAssignEducatorsGetVm CreateManageAssignEducatorsVm(Guid educationId)
        {
            var education = _unitOfWork.Education.GetById(educationId);
            var model = new Core.ViewModels.areas.admin.educator.ManageAssignEducatorsGetVm
            {
                EducationId = educationId,
                EducationName = education.Name,
                Educators = _unitOfWork.Educator.Get(null, x => x.OrderBy(o => o.User.Name), x => x.User)
            };
            return model;
        }
    }
}
