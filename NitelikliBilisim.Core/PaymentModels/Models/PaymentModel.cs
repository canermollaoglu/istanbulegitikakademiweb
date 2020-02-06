using System;
using System.Collections.Generic;

namespace NitelikliBilisim.Core.PaymentModels
{
    public class PaymentModel
    {
        public TransactionType TransactionType { get; set; }
        public string ConversationId { get; set; }
        public string HtmlContent { get; set; }
        public string Status { get; set; }
        public string Locale { get; set; }
        public PaymentModelSuccess Success { get; set; }
        public PaymentModelError Error { get; set; }

    }
    public class PaymentModelError
    {
        public string ErrorCode { get; set; }
        public string ErrorGroup { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class PaymentModelSuccess
    {
        public Guid InvoiceId { get; set; }
        public List<Guid> InvoiceDetailIds { get; set; }
    }
}
