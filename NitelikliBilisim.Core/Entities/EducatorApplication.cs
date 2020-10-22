using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.Entities
{
    public class EducatorApplication:BaseEntity<Guid>
    {
        public EducatorApplication()
        {
            Id = Guid.NewGuid();
        }

        [Required, MaxLength(100)]
        public string NameSurname { get; set; }
        [Required, MaxLength(15)]
        public string Phone { get; set; }
        [Required, MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(500)]
        public string Note { get; set; }
        public string CvUrl { get; set; }

    }
}
