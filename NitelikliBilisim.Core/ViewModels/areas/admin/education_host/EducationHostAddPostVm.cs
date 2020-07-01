using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.areas.admin.education_host
{
    public class EducationHostAddPostVm
    {
        [Required(ErrorMessage ="Şehir alanı boş geçilemez.")]
        public int? City { get; set; }
        [MaxLength(2048),Required(ErrorMessage ="Adres alanı boş geçilemez.")]
        public string Address { get; set; }
        [MaxLength(256)]
        [Required(ErrorMessage ="Kurum Adı alanı boş geçilemez.")]
        public string HostName { get; set; }
        [MaxLength(128)]
        public string Latitude { get; set; }
        [MaxLength(128)]
        public string Longitude { get; set; }
    }
}
