using System.Collections.Generic;

namespace NitelikliBilisim.Core.PaymentModels
{
    public class PaymentCompletionModel
    {
        public PaymentCompletionInvoice Invoice { get; set; }
        public List<PaymentCompletionInvoiceDetail> InvoiceDetails { get; set; }
    }
    public class PaymentCompletionInvoice
    {
        public string PaymentId { get; set; }
        public string BinNumber { get; set; }
        public string LastFourDigit { get; set; }
        public string HostRef { get; set; }
        public decimal CommissonFee { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal PaidPrice { get; set; }
    }
    public class PaymentCompletionInvoiceDetail
    {
        public string TransactionId { get; set; }
        public decimal Price { get; set; }
        public decimal CommissionFee { get; set; }
        public decimal CommisionRate { get; set; }
        public decimal MerchantPayout { get; set; }
        public decimal PaidPrice { get; set; }
    }
}
