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
        public bool IsIndividual { get; set; }
        public string BillingType { get; set; }
        public int PaymentCount { get; set; }
        public string TransactionStatus { get; set; }
        
    }
    public class _InvoiceDetail
    {
        public string Education { get; set; }
        public decimal PaidPriceNumeric { get; set; }
        public string PaidPriceText { get; set; }
        public bool IsCancelled { get; set; }
    }
    public class _CompanyInfo
    {
        public string CompanyName { get; set; }
    }
}
