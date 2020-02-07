using NitelikliBilisim.Core.Abstracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NitelikliBilisim.Core.Entities
{
    [Table("OnlinePaymentInfos")]
    public class OnlinePaymentInfo : BaseEntity<Guid>
    {
        [ForeignKey("Id")]
        public virtual Invoice Invoice { get; set; }

        [MaxLength(32)]
        public string PaymentId { get; set; }
        [MaxLength(12)]
        public string BinNumber { get; set; }
        [MaxLength(8)]
        public string LastFourDigit { get; set; }
        [MaxLength(32)]
        public string HostRef { get; set; }
        public decimal CommissonFee { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal PaidPrice { get; set; }
    }
}
