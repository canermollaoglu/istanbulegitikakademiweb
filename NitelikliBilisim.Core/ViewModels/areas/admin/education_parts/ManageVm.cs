using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_parts
{
    public class ManageVm
    {
        public Guid EducationId { get; set; }
        public string EducationName { get; set; }
        public List<_EducationPart> BaseParts { get; set; }
    }

    public class AddPartVm
    {
        public Guid EducationId { get; set; }
        public Guid? BasePartId { get; set; }
        [Required(ErrorMessage = "Sıra boş geçilemez"), Range(1, 100, ErrorMessage = "Sıra 1 ile 100 arasında bir değer olmalıdır")]
        public byte? Order { get; set; }
        [Required(ErrorMessage = "Başlık boş geçilemez")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Süre boş geçilemez"), Range(1, 255, ErrorMessage = "Süre 1 ile 255 arasında bir değer olmalıdır")]
        public byte? Duration { get; set; }
    }

    public class GetEducationPartsVm
    {
        public List<_EducationPart> EducationParts { get; set; }
    }

    public class _EducationPart
    {
        public Guid Id { get; set; }
        public Guid EducationId { get; set; }
        public string Title { get; set; }
        public byte Order { get; set; }
        public byte Duration { get; set; }
    }
}
