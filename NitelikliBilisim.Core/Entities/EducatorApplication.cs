using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("EducatorApplications")]
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
        public bool IsViewed { get; set; }
    }
}
