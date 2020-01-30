using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Tickets")]
    public class Ticket : BaseEntity<Guid>
    {
        public Ticket()
        {
            Id = Guid.NewGuid();
        }

        public bool IsUsed { get; set; }
        [MaxLength(450), ForeignKey("Owner")]
        public string OwnerId { get; set; }
        public virtual Customer Owner { get; set; }
        [ForeignKey("Host")]
        public Guid HostId { get; set; }
        public virtual EducationHost Host { get; set; }
        public Guid InvoiceDetailsId { get; set; }
    }
}
