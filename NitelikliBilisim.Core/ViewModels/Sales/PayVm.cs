﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Iyzipay.Model;

namespace NitelikliBilisim.Core.ViewModels.Sales
{
    public class PayPostVm
    {
        [Required]
        public _CardInfo CardInfo { get; set; }
        [Required]
        public _InvoiceInfo InvoiceInfo { get; set; }
        public _CorporateInvoiceInfo CorporateInvoiceInfo { get; set; }
        [Required]
        public bool IsDistantSalesAgreementConfirmed { get; set; }
        public List<Guid> CartItems { get; set; }
        public PaymentChannel PaymentChannel { get; set; } = PaymentChannel.MOBILE_WEB;
        public PaymentGroup PaymentGroup { get; set; } = PaymentGroup.PRODUCT;
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

    public enum CreditCardTypes
    {
        TROY = 10,
        MASTERCARD = 100,
        VISA = 200,
        AMEX = 1000
    }

    public enum CreditCardPrograms
    {
        BONUS = 10,
        WORD = 20,
        MAXIMUM = 30,
        AXESS = 40,
        CARDFINANS = 50,
        PARAF = 60,
        ADVANTAGE = 70
    }

}
