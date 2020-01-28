using NitelikliBilisim.Core.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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

        [ForeignKey("Invoice")]
        public Guid InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}
