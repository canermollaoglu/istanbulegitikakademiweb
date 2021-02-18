using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.ViewModels.Main.EducatorApplication
{
    public class EducatorApplicationAddVm
    {
        [Required,MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(500)]
        public string Note { get; set; }
        [Required, MaxLength(15)]
        public string Phone { get; set; }
        [Required, MaxLength(100)]
        public string NameSurname { get; set; }
        [Required]
        public IFormFile Cv { get; set; }
    }
    
}
