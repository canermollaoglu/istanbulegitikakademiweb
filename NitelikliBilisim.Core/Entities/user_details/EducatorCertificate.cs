using NitelikliBilisim.Core.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.user_details
{
    [Table("EducatorCertificates")]
    public class EducatorCertificate : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CertificateImagePath { get; set; }
    }
}
