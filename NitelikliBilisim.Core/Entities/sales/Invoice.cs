using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Invoices")]
    public class Invoice : BaseEntity<Guid>
    {
        public Invoice()
        {
            Id = Guid.NewGuid();
        }

        public CustomerType BillingType { get; set; }
        public string TaxNo { get; set; }
        public string TaxOffice { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Earning { get; set; }
        public bool IsCash { get; set; }
        public byte PaymentCount { get; set; }

        [ForeignKey("Customer"), MaxLength(450)]
        public string CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
