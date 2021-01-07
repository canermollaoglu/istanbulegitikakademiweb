using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class UpdateNBUYEducationInfoVm
    {
        [Required(ErrorMessage ="Eğitim Merkezi alanı zorunludur.")]
        public EducationCenter EducationCenter { get; set; }
        [Required(ErrorMessage ="Eğitim Başlangıç Tarihi alanı zorunludur.")]
        public DateTime StartAt { get; set; }
        [Required(ErrorMessage = "Eğitim Türü alanı zorunludur.")]
        public Guid EducationCategoryId { get; set; }
        public string UserId { get; set; }
    }
}
