using System;
using System.Collections.Generic;
using System.Text;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class MyInvoicesVm
    {
        public _Invoice Invoice { get; set; }
        public List<_InvoiceDetail> InvoiceDetails { get; set; }
    }

    public class _Invoice
    {
        public Guid InvoiceId { get; set; }
        public bool IsIndividual { get; set; }
        public string BillingType { get; set; }
        public int PaymentCount { get; set; }
        public string TransactionStatus { get; set; }
        public bool IsEligibleToFullyCancel { get; set; }
        public DateTime CreatedDate { get; set; } // TODO: gün içerisinde alınmış olma durumunu kontrol etmek için (yeterli olup olmadığı kontrol edilmeli)
        public _CompanyInfo CompanyInfo { get; set; }
    }
    public class _InvoiceDetail
    {
        public Guid InvoiceDetailsId { get; set; }
        public string Education { get; set; }
        public decimal PaidPriceNumeric { get; set; }
        public string PaidPriceText { get; set; }
        public bool IsCancelled { get; set; }
        public _CorrespondingGroup Group { get; set; }
    }
    public class _CorrespondingGroup
    {
        public string GroupName { get; set; }
        public bool IsGroupStarted { get; set; }
        public DateTime StartDate { get; set; }
        public string StartDateText { get; set; }
    }
    public class _CompanyInfo
    {
        public string CompanyName { get; set; }
    }
}
