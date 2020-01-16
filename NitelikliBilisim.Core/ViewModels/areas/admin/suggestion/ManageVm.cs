using NitelikliBilisim.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.suggestion
{
    public class ManageGetVm
    {
        public List<EducationCategory> Categories { get; set; }
    }

    public class AddPostVm
    {
        [Required(ErrorMessage = "Minimum boş geçilemez")]
        public byte? MinRange { get; set; }
        [Required(ErrorMessage = "Maksimum boş geçilemez")]
        public byte? MaxRange { get; set; }
        [Required(ErrorMessage = "Kategori seçimi zorunludur")]
        public Guid? CategoryId { get; set; }
        public List<Guid> SuggestableEducations { get; set; }
    }

    public class GetSuggestionsVm
    {
        public List<_Suggestion> Suggestions { get; set; }
    }

    public class _Suggestion
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public byte Min { get; set; }
        public byte Max { get; set; }
        public string SuggestableEducations { get; set; }
    }
}
