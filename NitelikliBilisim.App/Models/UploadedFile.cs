using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.App.Models
{
    public class UploadedFile
    {
        public string Base64Content { get; set; }
        [Required(ErrorMessage = "Uzantı zorunludur")]
        public string Extension { get; set; }
        public string PreDeterminedName { get; set; }
    }
}
