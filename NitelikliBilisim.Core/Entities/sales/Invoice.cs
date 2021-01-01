using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Enums;
using NitelikliBilisim.Core.PaymentModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("Invoices")]
    public class Invoice : BaseEntity<Guid>, IAuditIp
    {
        public Invoice()
        {
            Id = Guid.NewGuid();
        }

        public CustomerType BillingType { get; set; }
        public TransactionStatus TransactionStatus { get; set; }

        [MaxLength(128)]
        public string CompanyName { get; set; }
        [MaxLength(32)]
        public string TaxNo { get; set; }
        [MaxLength(32)]
        public string TaxOffice { get; set; }
        public byte PaymentCount { get; set; }

        [ForeignKey("Customer"), MaxLength(450)]
        public string CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        [MaxLength(32)]
        public string CreatedIp { get; set; }
        [MaxLength(32)]
        public string UpdatedIp { get; set; }
        public string InvoicePdfUrl { get; set; }

        public virtual List<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual OnlinePaymentInfo OnlinePaymentInfo { get; set; }
    }
}
