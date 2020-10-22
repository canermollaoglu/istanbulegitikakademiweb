using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("CorporateMembershipApplications")]
    public class CorporateMembershipApplication : BaseEntity<Guid>
    {
        public CorporateMembershipApplication()
        {
            Id = Guid.NewGuid();
        }

        [Required, MaxLength(150)]
        public string CompanyName { get; set; }
        [Required, MaxLength(100)]
        public string CompanySector { get; set; }
        [Required, MaxLength(100)]
        public string NameSurname { get; set; }
        [Required, MaxLength(15)]
        public string Phone { get; set; }
        [Required, MaxLength(100)]
        public string Department { get; set; }
        public int NumberOfEmployees { get; set; }
        public string Address { get; set; }
        public string RequestNote { get; set; }
        public bool IsViewed { get; set; }
    }
}
