using Iyzipay.Model;
using NitelikliBilisim.Business.UoW;
using NitelikliBilisim.Core.PaymentModels;
using NitelikliBilisim.Core.Services.Payments;
using NitelikliBilisim.Core.ViewModels.Sales;
using System;
using System.Collections.Generic;

namespace NitelikliBilisim.Business.PaymentFactory
{
    public class PaymentManager
    {
        private readonly IPayer _payer;
        private readonly TransactionType _transactionType;
        public PaymentManager(IPaymentService service, TransactionType transactionType)
        {
            _payer = CreatePayer(service, transactionType);
        }

        public PaymentModel Pay(UnitOfWork unitOfWork, PayData data)
        {
            return _payer.Pay(unitOfWork, data);
        }
        public PaymentCompletionModel Create3dCompletionModel(ThreedsPayment result)
        {
            if (result.Status == "failure")
                return null;

            var model = new PaymentCompletionModel
            {
                Invoice = new PaymentCompletionInvoice
                {
                    BinNumber = result.BinNumber,
                    CommissionRate = Convert.ToDecimal(result.IyziCommissionRateAmount),
                    CommissonFee = Convert.ToDecimal(result.IyziCommissionFee),
                    HostRef = result.HostReference,
                    LastFourDigit = result.LastFourDigits,
                    PaidPrice = Convert.ToDecimal(result.PaidPrice),
                    PaymentId = result.PaymentId
                }
            };
            model.InvoiceDetails = new List<PaymentCompletionInvoiceDetail>();
            foreach (var item in result.PaymentItems)
                model.InvoiceDetails.Add(new PaymentCompletionInvoiceDetail
                {
                    TransactionId = item.PaymentTransactionId,
                    CommisionRate = Convert.ToDecimal(item.IyziCommissionRateAmount),
                    CommissionFee = Convert.ToDecimal(item.IyziCommissionFee),
                    MerchantPayout = Convert.ToDecimal(item.MerchantPayoutAmount),
                    PaidPrice = Convert.ToDecimal(item.PaidPrice),
                    Price = Convert.ToDecimal(item.Price)
                });

            return model;
        }

        private IPayer CreatePayer(IPaymentService service, TransactionType type)
        {
            switch (type)
            {
                case TransactionType.Normal:
                    return new NormalPayer(service);
                case TransactionType.Secure3d:
                    return new Secure3dPayer(service);
                case TransactionType.BKM:
                    return new BkmPayer(service);
                default:
                    return new NormalPayer(service);
            }
        }
    }
}
