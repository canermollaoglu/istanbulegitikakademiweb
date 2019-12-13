using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_media_items;
using NitelikliBilisim.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Areas.Admin.VmCreator.EducationMediaItems
{
    public class EducationMediaItemVmCreator
    {
        private readonly UnitOfWork _unitOfWork;
        public EducationMediaItemVmCreator(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ManageVm CreateManageVm(Guid educationId)
        {
            var education = _unitOfWork.Education.GetById(educationId);
            return new ManageVm
            {
                EducationId = education.Id,
                EducationName = education.Name,
                MediaItemTypes = EnumSupport.ToKeyValuePair<EducationMediaType>()
            };
        }

        public GetEducationMediaItemsVm CreateEducationMediaItemsVm(Guid educationId)
        {
            var medias = _unitOfWork.EducationMedia.Get(x => x.EducationId == educationId);

            var educationMediaItems = new List<_EducationMediaItem>();
            foreach (var item in medias)
                educationMediaItems.Add(new _EducationMediaItem
                {
                    Id = item.Id,
                    EducationId = item.EducationId,
                    MediaItemType = item.MediaType,
                    FileUrl = item.FileUrl
                });

            return new GetEducationMediaItemsVm
            {
                EducationMediaItems = educationMediaItems
            };
        }
    }
}
