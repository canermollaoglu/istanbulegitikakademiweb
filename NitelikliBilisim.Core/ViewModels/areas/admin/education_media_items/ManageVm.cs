using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_media_items
{
    public class ManageVm
    {
        public Guid EducationId { get; set; }
        public string EducationName { get; set; }
        public Dictionary<int, string> MediaItemTypes { get; set; }
    }

    public class AddMediaItemVm
    {
        public Guid EducationId { get; set; }
        public int MediaItemType { get; set; }
    }

    public class GetEducationMediaItemsVm
    {
        public List<_EducationMediaItem> EducationMediaItems { get; set; }
    }

    public class _EducationMediaItem
    {
        public Guid Id { get; set; }
        public Guid EducationId { get; set; }
        public EducationMediaType MediaItemType { get; set; }
        public string FileUrl { get; set; }
    }
}
