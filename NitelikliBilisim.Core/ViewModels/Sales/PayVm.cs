using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Iyzipay.Model;
using NitelikliBilisim.Core.Services.Payments;

namespace NitelikliBilisim.Core.ViewModels.Sales
{
    public class PayPostVm
    {
        public Guid BasketId { get; set; }
        public Guid ConversationId { get; set; } = Guid.NewGuid();
        [Required]
        public _CardInfo CardInfo { get; set; }
        [Required]
        public _InvoiceInfo InvoiceInfo { get; set; }
        public _CorporateInvoiceInfo CorporateInvoiceInfo { get; set; }
        public _PaymentInfo PaymentInfo { get; set; } = new _PaymentInfo();
        public _SpecialInfo SpecialInfo { get; set; } = new _SpecialInfo();
        public bool IsDistantSalesAgreementConfirmed { get; set; }
        public string CartItemsJson { get; set; }
        public List<Guid> CartItems { get; set; }

    }

    public class _CardInfo
    {
        public string NameOnCard { get; set; }

        public string NumberOnCard { get; set; }

        [Required, MaxLength(2)]
        public string MonthOnCard { get; set; }

        [Required, MaxLength(2)]
        public string YearOnCard { get; set; }

        [Required, MinLength(3), MaxLength(3)]
        public string CVC { get; set; }
    }
    public class _InvoiceInfo
    {
        [Required, MaxLength(32)]
        public string City { get; set; }

        [Required, MaxLength(32)]
        public string Town { get; set; }

        [Required, MaxLength(256)]
        public string Address { get; set; }

        [Required, MaxLength(16)]
        public string Phone { get; set; }
        public bool IsIndividual { get; set; }
    }
    public class _CorporateInvoiceInfo
    {
        [MaxLength(256)]
        public string CompanyName { get; set; }

        [MaxLength(256)]
        public string TaxNo { get; set; }

        [MaxLength(256)]
        public string TaxOffice { get; set; }
    }
    public class _PaymentInfo
    {
        public PaymentChannel PaymentChannel { get; set; } = PaymentChannel.WEB;
        public PaymentGroup PaymentGroup { get; set; } = PaymentGroup.PRODUCT;
        public byte Installments { get; set; } = 1;
    }
    public class _SpecialInfo
    {
        public string Ip { get; set; }
        public string IdentityNumber { get; set; } = "12345678901";
    }
}
