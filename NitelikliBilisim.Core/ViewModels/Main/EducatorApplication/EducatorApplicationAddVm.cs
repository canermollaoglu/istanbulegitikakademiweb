using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.ViewModels.Main.EducatorApplication
{
    public class EducatorApplicationAddVm
    {
        [Required]
        public string Email { get; set; }
        public string Note { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string NameSurname { get; set; }
        [Required]
        public IFormFile Cv { get; set; }
    }
    
}
