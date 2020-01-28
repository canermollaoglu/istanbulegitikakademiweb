using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("InvoiceDetails")]
    public class InvoiceDetail : BaseEntity<Guid>
    {
        public InvoiceDetail()
        {
            Id = Guid.NewGuid();
        }

        public Guid EducationId { get; set; }
        public decimal PriceAtCurrentDate { get; set; }
        public bool IsUsedAsTicket { get; set; }

        [ForeignKey("Invoice")]
        public Guid InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}
