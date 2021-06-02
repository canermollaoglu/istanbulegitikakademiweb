using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NitelikliBilisim.Core.Enums;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_component
{
    public class EducationComponentAddPostVm
    {
        [Required(ErrorMessage = "Komponent tipi alanı boş geçilemez.")]
        public EducationComponentType ComponentType  { get; set; }
        [Required(ErrorMessage = "Önerilecek kişi tipi alanı boş geçilemez.")]
        public EducationComponentSuggestionType SuggestionType { get; set; }
        [Required(ErrorMessage = "Sıra alanı boş geçilemez.")]
        public int Order { get; set; }
        [Required(ErrorMessage = "Eğitim alanı boş geçilemez.")]
        public Guid EducationId{ get; set; }


    }
}
