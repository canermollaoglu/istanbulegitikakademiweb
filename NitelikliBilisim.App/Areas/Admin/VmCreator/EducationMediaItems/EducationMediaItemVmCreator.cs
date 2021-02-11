using MUsefulMethods;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.Services.Abstracts;
using NitelikliBilisim.Core.ViewModels.areas.admin.education_media_items;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.App.Areas.Admin.VmCreator.EducationMediaItems
{
    public class EducationMediaItemVmCreator
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IStorageService _storageService; 
        public EducationMediaItemVmCreator(UnitOfWork unitOfWork, IStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
        }

        public ManageVm CreateManageVm(Guid educationId)
        {
            var education = _unitOfWork.Education.GetById(educationId);
            return new ManageVm
            {
                EducationId = education.Id,
                EducationName = education.Name,
                MediaItemTypes = EnumHelpers.ToKeyValuePair<EducationMediaType>()
            };
        }

        public GetEducationMediaItemsVm CreateEducationMediaItemsVm(Guid educationId)
        {
            var medias = _unitOfWork.EducationMedia.Get(x => x.EducationId == educationId);

            var educationMediaItems = new List<_EducationMediaItem>();
            foreach (var item in medias)
            {
                var media = new _EducationMediaItem
                {
                    Id = item.Id,
                    EducationId = item.EducationId,
                    MediaItemType = EnumHelpers.GetDescription(item.MediaType)
                };

                try
                {
                    media.FileUrl = _storageService.BlobUrl+item.FileUrl;
                }
                catch
                {
                }

                educationMediaItems.Add(media);
            }

            return new GetEducationMediaItemsVm
            {
                EducationMediaItems = educationMediaItems
            };
        }
    }
}
