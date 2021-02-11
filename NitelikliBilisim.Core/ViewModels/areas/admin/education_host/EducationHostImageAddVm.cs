using System;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_host
{
    public class EducationHostImageAddVm
    {
        public Guid EducationHostId { get; set; }
        public _PostedFile PostedFile { get; set; }
    }


    public class _PostedFile
    {
        [Required(ErrorMessage = "Dosya içeriği boş olamaz")]
        public string Base64Content { get; set; }
        [Required(ErrorMessage = "Dosya uzantısı boş olamaz")]
        public string Extension { get; set; }
    }
}
