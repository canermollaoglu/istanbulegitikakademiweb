using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("OnlinePaymentDetailsInfos")]
    public class OnlinePaymentDetailsInfo : BaseEntity<Guid>
    {
        [ForeignKey("Id")]
        public virtual InvoiceDetail InvoiceDetail { get; set; }
        [MaxLength(32)]
        public string TransactionId { get; set; }
        public decimal Price { get; set; }
        public decimal CommissionFee { get; set; }
        public decimal CommisionRate { get; set; }
        public decimal MerchantPayout { get; set; }
        public decimal PaidPrice { get; set; }
        public DateTime BlockageResolveDate { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime? CancellationDate { get; set; }
    }
}
