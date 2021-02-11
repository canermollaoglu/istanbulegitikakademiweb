using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities.user_details
{
    [Table("CustomerCertificates")]
    public class CustomerCertificate : BaseEntity<Guid>
    {
        public CustomerCertificate()
        {
            Id = Guid.NewGuid();
        }
        public string CustomerId { get; set; }
        public Guid GroupId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }
    }
}
