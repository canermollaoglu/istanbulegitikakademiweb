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

        [MaxLength(128)]
        public string CompanyName { get; set; }
        [MaxLength(32)]
        public string TaxNo { get; set; }
        [MaxLength(32)]
        public string TaxOffice { get; set; }
        public bool IsCash { get; set; }
        public byte PaymentCount { get; set; }
        public Guid ConversationId { get; set; }
        [MaxLength(16)]
        public string PaymentId { get; set; }

        [ForeignKey("Customer"), MaxLength(450)]
        public string CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
