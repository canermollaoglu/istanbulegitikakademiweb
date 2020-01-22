using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        public _PostedFile PostedFile { get; set; }
    }

    public class GetEducationMediaItemsVm
    {
        public List<_EducationMediaItem> EducationMediaItems { get; set; }
    }

    public class _EducationMediaItem
    {
        public Guid Id { get; set; }
        public Guid EducationId { get; set; }
        public string MediaItemType { get; set; }
        public string FileUrl { get; set; }
    }

    public class _PostedFile
    {
        [Required(ErrorMessage = "Dosya içeriği boş olamaz")]
        public string Base64Content { get; set; }
        [Required(ErrorMessage = "Dosya uzantısı boş olamaz")]
        public string Extension { get; set; }
    }
}
