using NitelikliBilisim.App.Areas.Admin.Models.Education;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.areas.admin.education;
using NitelikliBilisim.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var categories = _unitOfWork.EducationCategory.Get(null, x => x.OrderBy(o => o.Name));
            var levels = EnumSupport.ToKeyValuePair<EducationLevel>();
            return new AddGetVm
            {
                Categories = categories,
                Levels = levels
            };
        }

        public ListGetVm CreateListGetVm(int page = 0, int shownRecords = 15)
        {
            return _unitOfWork.Education.GetPagedEducations(page, shownRecords);
        }

        public UpdateGetVm CreateUpdateGetVm(Guid educationId)
        {
            var addGetVm = CreateAddGetVm();
            var education = _unitOfWork.Education.GetById(educationId);
            var relatedCategories = _unitOfWork.Education.GetCategories(educationId);
            return new UpdateGetVm
            {
                Categories = addGetVm.Categories,
                Levels = addGetVm.Levels,
                Education = education,
                RelatedCategories = relatedCategories
            };
        }

        public void SendVmToUpdate(UpdatePostVm data)
        {
            _unitOfWork.Education.Update(new Core.Entities.Education
            {
                Id = data.EducationId,
                Days = data.Days.Value,
                HoursPerDay = data.HoursPerDay.Value,
                Description = data.Description,
                Level = (EducationLevel)data.EducationLevel,
                Name = data.Name,
                NewPrice = data.Price,
                IsActive = data.IsActive
            }, data.CategoryIds);
        }
    }
}
