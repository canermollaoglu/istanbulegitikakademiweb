using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Sales")]
    public class Sale : BaseEntity<Guid>
    {
        public Sale()
        {
            Id = Guid.NewGuid();
        }

        public CustomerType BillingType { get; set; }
        public string TaxNo { get; set; }
        public string TaxOffice { get; set; }
        public Guid EducationId { get; set; }
        public decimal Paid { get; set; }
        public decimal PriceAtCurrentDate { get; set; }
        public decimal Earning { get; set; }
        public bool IsCash { get; set; }
        public byte PaymentCount { get; set; }

        [ForeignKey("Customer"), MaxLength(128)]
        public string CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
